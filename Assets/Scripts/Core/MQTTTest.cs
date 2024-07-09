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

        // MQTT 이벤트 핸들러 설정
        client.UseConnectedHandler(async e =>
        {
            Debug.Log("Connected to MQTT broker.");

            // 특정 토픽 구독
            await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(mTopic).Build());
            Debug.Log("Subscribed to topic: " + mTopic);

            // 메시지 발행
            await PublishMessage("MQTT Test published from Unity!!!");
        });

        client.UseDisconnectedHandler(e =>
        {
            Debug.Log("Disconnected from MQTT broker.");
        });

        client.UseApplicationMessageReceivedHandler(e =>
        {
            // 수신된 메시지 처리
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
        subscribedMessagesText.ForceMeshUpdate(); // 텍스트 메시 업데이트 강제 수행
        LayoutRebuilder.ForceRebuildLayoutImmediate(subscribedMessagesText.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(subscribedMessagesText.transform.parent.GetComponent<RectTransform>());
    }

    async void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 클라이언트 연결 해제
        if (client != null && client.IsConnected)
        {
            await client.DisconnectAsync();
        }
    }
}
