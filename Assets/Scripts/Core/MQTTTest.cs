using UnityEngine;
using UnityEngine.UI;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Text;
using System.Threading.Tasks;
using System;
using TMPro;
using System.Collections;

public class MQTTnetClient : MonoBehaviour
{
    private IMqttClient client;
    private IMqttClientOptions options;

    public TMP_InputField clientIdInputField;
    public TMP_InputField serverAddressInputField;
    public TMP_InputField portInputField;
    public TMP_InputField topicInputField;
    public TMP_InputField payloadInputField;
    public Button connectButton;
    public Button disconnectButton;
    public Button publishButton;
    public TextMeshProUGUI subscribedMessagesText;

    public RectTransform ChatContent;    
    public ScrollRect ChatScrollRect;

    private string mClientID;
    private string mTcpServer;
    private int mPort;
    private string mTopic;

    async void Start()
    {
        connectButton.onClick.AddListener(OnConnectButtonClicked);
        disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
        publishButton.onClick.AddListener(OnPublishButtonClicked);

        var factory = new MqttFactory();
        client = factory.CreateMqttClient();

        // MQTT �̺�Ʈ �ڵ鷯 ����
        client.UseConnectedHandler(async e =>
        {
            Debug.Log("Connected to MQTT broker.");

            // Ư�� ���� ����
            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(mTopic).Build());
            Debug.Log("Subscribed to topic: " + mTopic);

            // �޽��� ����
            await PublishMessage("MQTT Test published from Unity!!!");
        });

        client.UseDisconnectedHandler(e =>
        {
            Debug.Log("Disconnected from MQTT broker.");
        });

        client.UseApplicationMessageReceivedHandler(e =>
        {
            // ���ŵ� �޽��� ó��
            string receivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            Debug.Log("Received message: " + receivedMessage);
            UpdateSubscribedMessages(receivedMessage);
        });
    }

    private async void OnConnectButtonClicked()
    {
        mClientID = clientIdInputField.text;
        mTcpServer = serverAddressInputField.text;
        mPort = int.Parse(portInputField.text);
        mTopic = topicInputField.text;

        options = new MqttClientOptionsBuilder()
            .WithClientId(mClientID)
            .WithTcpServer(mTcpServer, mPort)
            .WithCleanSession()
            .Build();

        await ConnectClient();
    }

    private async void OnDisconnectButtonClicked()
    {
        if (client != null && client.IsConnected)
        {
            await client.DisconnectAsync();
        }
    }

    private async void OnPublishButtonClicked()
    {
        string payload = payloadInputField.text;
        await PublishMessage(payload);
    }

    private async Task ConnectClient()
    {
        try
        {
            await client.ConnectAsync(options);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error connecting to MQTT broker: " + ex.Message);
        }
    }

    private async Task PublishMessage(string message)
    {
        var messagePayload = new MqttApplicationMessageBuilder()
            .WithTopic(mTopic)
            .WithPayload(message)
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        await client.PublishAsync(messagePayload);
    }

    private void UpdateSubscribedMessages(string message)
    {
        subscribedMessagesText.text += message + "\n";
        //StartCoroutine(ForceUpdateUI());
        Fit(subscribedMessagesText.GetComponent<RectTransform>());
        Fit(ChatContent);
        Invoke("ScrollDelay", 0.03f);
    }
    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    void ScrollDelay() => ChatScrollRect.verticalScrollbar.value = 0;

    private IEnumerator ForceUpdateUI()
    {
        yield return new WaitForEndOfFrame();
        subscribedMessagesText.ForceMeshUpdate(); // �ؽ�Ʈ �޽� ������Ʈ ���� ����
        LayoutRebuilder.ForceRebuildLayoutImmediate(subscribedMessagesText.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(subscribedMessagesText.transform.parent.GetComponent<RectTransform>());
    }

    async void OnApplicationQuit()
    {
        // ���ø����̼� ���� �� Ŭ���̾�Ʈ ���� ����
        if (client != null && client.IsConnected)
        {
            await client.DisconnectAsync();
        }
    }
}
