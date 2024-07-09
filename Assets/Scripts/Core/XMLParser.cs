using UnityEngine;
using System.Xml;
using System.Data;
using System;
using System.Collections.Generic;

public class XMLParser : MonoBehaviour
{
    [Header("protocol 정보")]
    public string protocol_Version = string.Empty;  // 프로토콜 버전
    public string protocol_Name = string.Empty;     // 프로토콜명
    public string protocol_Key = string.Empty;      // 프로토콜키
    public string protocol_FWCode = string.Empty;   // 펌웨어코드
    public string protocol_Start = string.Empty;    // Modbus 시작주소
    public string protocol_End = string.Empty;      // Modbus 끝 주소
    public string protocol_Affliation = string.Empty; // 프로토콜 작성자 소속
    public string protocol_Uploader = string.Empty; // 프로토콜 작성자명
    public string protocol_Remark = string.Empty;   // 비고
    public string xmlContent = string.Empty;

    // singleton 패턴을 위한 변수
    private static XMLParser _instance;

    public static XMLParser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<XMLParser>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "XMLParser";
                    _instance = obj.AddComponent<XMLParser>();
                }
            }
            return _instance;
        }
    }

    // 프로토콜키에 맞는 XML 데이터를 가져와 변수에 할당
    public void GetXML(string protocolKey)
    {
        DataRow[] foundRows = ClientDatabase.SelectRowsFromTable(ClientDatabase.FetchProtocolList(), "TBL_PROTOCOL_LIST", $"`KEY` = '{protocolKey}'");        
        if(foundRows.Length > 0)
        {
            xmlContent = foundRows[0]["XML"].ToString();            
        }
        else
        {
            //Debug.LogError("XMLParser : No xml data could be found for the conditions.");
            return;
        }        

        ParseStaticXMLData(xmlContent);        
    }

    // 정적 XML 요소 값 할당(Protocol 태그에 한함)
    public void ParseStaticXMLData(string xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            //Debug.LogError("XMLParser : XML Content is empty or null.");
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();

        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch (XmlException ex)
        {
            //Debug.LogError($"XMLParser : Error while parsing XML: {ex.Message}");
            return;
        }

        // protocol 정보 가져오기
        XmlNode protocolNode = xmlDoc.SelectSingleNode("/protocol");
        protocol_Version = protocolNode.Attributes["version"].Value; // 프로토콜 버전
        protocol_Name = protocolNode.Attributes["name"].Value;   // 프로토콜명
        protocol_Key = protocolNode.Attributes["key"].Value; // 프로토콜키
        protocol_FWCode = protocolNode.Attributes["fw_code"].Value; // 펌웨어코드
        protocol_Start = protocolNode.Attributes["start"].Value; // Modbus 시작주소
        protocol_End = protocolNode.Attributes["end"].Value; // Modbus 끝 주소
        protocol_Affliation = protocolNode.Attributes["affliation"].Value; // 프로토콜 작성자 소속
        protocol_Uploader = protocolNode.Attributes["uploader"].Value; // 프로토콜 작성자명
        protocol_Remark = protocolNode.Attributes["remark"].Value; // 비고        
    }

    // group 태그들 중 alt 속성이 trend인 group 태그의 하위 태그와 속성, 값을 읽음
    public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetTrendAltAttributes(string xml)
    {
        // 결과를 저장할 딕셔너리 초기화
        var result = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        // XML 내용이 비어있으면 오류 메시지 출력 후 Null 반환
        if (string.IsNullOrEmpty(xml))
        {
            //Debug.LogError("XMLParser : XML content is empty");
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();

        // XML 문서 로드
        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch (XmlException ex)
        {
            //Debug.LogError($"XMLParser : Error parsing XML: {ex.Message}");
            return null;
        }

        // alt 속성이 "trend"인 group 태그 가져오기
        XmlNodeList altTrendGroupNodes = xmlDoc.SelectNodes("/protocol/group[@alt='trend']");
        if (altTrendGroupNodes == null || altTrendGroupNodes.Count == 0)
        {
            //Debug.LogError("XMLParser : No 'group' tag found with alt='trend'.");
            return null;
        }

        // 각 group 태그에 대해
        foreach (XmlNode groupNode in altTrendGroupNodes)
        {
            var groupTitle = groupNode.Attributes["title"]?.Value;
            if (groupTitle == null) continue; // title 속성이 없으면 넘어감

            var tagsResult = new Dictionary<string, Dictionary<string, string>>();

            // 각 tag 태그에 대해
            foreach (XmlNode tagNode in groupNode.SelectNodes("tag"))
            {
                var tagAttributes = new Dictionary<string, string>();
                foreach (XmlAttribute attr in tagNode.Attributes)
                {
                    tagAttributes[attr.Name] = attr.Value;
                }

                var tagName = tagNode.Attributes["name"]?.Value;
                if (tagName == null) continue; // name 속성이 없으면 넘어감

                tagsResult[tagName] = tagAttributes;
            }

            result[groupTitle] = tagsResult;
        }

        // 저장된 속성들의 딕셔너리 반환
        return result;
    }

    // XML 문자열에서 protocol 태그의 정보와 group, tag 태그의 속성을 분석하여 반환하는 함수
    public Dictionary<string, object> GetAllXMLAttributes(string xml)
    {
        var result = new Dictionary<string, object>();
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(xml);

        // protocol 태그의 속성 분석
        XmlNode protocolNode = xmlDoc.SelectSingleNode("/protocol");
        if (protocolNode != null)
        {
            var protocolAttributes = new Dictionary<string, string>();
            foreach (XmlAttribute attr in protocolNode.Attributes)
            {
                protocolAttributes[attr.Name] = attr.Value;
            }
            result["protocol"] = protocolAttributes;

            // group 태그들의 분석
            var groups = new Dictionary<string, Dictionary<string, object>>();
            foreach (XmlNode groupNode in protocolNode.SelectNodes("group"))
            {
                var groupAttributes = new Dictionary<string, string>();
                foreach (XmlAttribute attr in groupNode.Attributes)
                {
                    groupAttributes[attr.Name] = attr.Value;
                }

                // tag 태그들의 분석
                var tags = new Dictionary<string, Dictionary<string, string>>();
                foreach (XmlNode tagNode in groupNode.SelectNodes("tag"))
                {
                    var tagAttributes = new Dictionary<string, string>();
                    foreach (XmlAttribute attr in tagNode.Attributes)
                    {
                        tagAttributes[attr.Name] = attr.Value;
                    }
                    if (tagAttributes.ContainsKey("name"))
                    {
                        tags[tagAttributes["name"]] = tagAttributes;
                    }
                }

                if (groupAttributes.ContainsKey("title"))
                {
                    groups[groupAttributes["title"]] = new Dictionary<string, object>
                    {
                        { "attributes", groupAttributes },
                        { "tags", tags }
                    };
                }
            }

            result["groups"] = groups;
        }

        return result;
    }

    public Dictionary<string, object> GetAllSystemAttributes(string xml, string iid, string cid)
    {
        var result = new Dictionary<string, object>();
        XmlDocument xmlDoc = new XmlDocument();

        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch(Exception e) {
            Debug.Log($"{iid}, {cid} : {e}");
        }

        // protocol 태그의 속성 분석
        XmlNode protocolNode = xmlDoc.SelectSingleNode("/protocol");
        if (protocolNode != null)
        {
            var protocolAttributes = new Dictionary<string, string>();
            foreach (XmlAttribute attr in protocolNode.Attributes)
            {
                protocolAttributes[attr.Name] = attr.Value;
            }
            result["protocol"] = protocolAttributes;

            // group 태그들의 분석
            var groups = new Dictionary<string, Dictionary<string, object>>();
            foreach (XmlNode groupNode in protocolNode.SelectNodes("group[@alt='system']"))
            {
                var groupAttributes = new Dictionary<string, string>();
                foreach (XmlAttribute attr in groupNode.Attributes)
                {
                    groupAttributes[attr.Name] = attr.Value;
                }

                // tag 태그들의 분석
                var tags = new Dictionary<string, Dictionary<string, string>>();
                foreach (XmlNode tagNode in groupNode.SelectNodes("tag"))
                {
                    var tagAttributes = new Dictionary<string, string>();
                    foreach (XmlAttribute attr in tagNode.Attributes)
                    {
                        tagAttributes[attr.Name] = attr.Value;
                    }
                    if (tagAttributes.ContainsKey("name"))
                    {
                        tags[tagAttributes["name"]] = tagAttributes;
                    }
                }

                if (groupAttributes.ContainsKey("title"))
                {
                    groups[groupAttributes["title"]] = new Dictionary<string, object>
                    {
                        { "attributes", groupAttributes },
                        { "tags", tags }
                    };
                }
            }

            result["groups"] = groups;
        }

        return result;
    }

    public Dictionary<string, Dictionary<string, string>> GetSetGroupAttributes(string xml)
    {
        // 결과를 저장할 딕셔너리 초기화
        Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

        // XML 내용이 비어있으면 오류 메시지 출력 후 Null 반환
        if (string.IsNullOrEmpty(xml))
        {
            //Debug.LogError("XMLParser : XML content is empty");
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();

        // XML 문서 로드
        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch (XmlException ex)
        {
            //Debug.LogError($"XML 파싱 중 오류 발생: {ex.Message}");
            return null;
        }

        // group 태그 가져오기
        XmlNodeList groupNodes = xmlDoc.SelectNodes("/protocol/group");
        if (groupNodes == null || groupNodes.Count == 0)
        {
            //Debug.LogError("group 태그가 없습니다.");
            return null;
        }

        // 각 group 태그에 대해
        foreach (XmlNode setGroup in groupNodes)
        {
            Dictionary<string, string> attributesDict = new Dictionary<string, string>();

            // 해당 태그의 모든 속성을 딕셔너리에 저장
            if (setGroup.Attributes != null)
            {
                foreach (XmlAttribute attr in setGroup.Attributes)
                {
                    attributesDict[attr.Name] = attr.Value;
                }
            }

            // 'title' 속성을 키로 사용해 전체 결과 딕셔너리에 추가
            result[attributesDict["title"]] = attributesDict;
        }

        // 저장된 속성들의 딕셔너리 반환
        return result;
    }

    public Dictionary<string, string> GetTagAttributesByAddr(string xml, string targetAddr)
    {
        // XML 내용이 비어있으면 오류 메시지 출력 후 Null 반환
        if (string.IsNullOrEmpty(xml))
        {
            //Debug.LogError("XML 내용이 비어 있습니다.");
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();

        // XML 문서 로드
        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch (XmlException ex)
        {
            //Debug.LogError($"XML 파싱 중 오류 발생: {ex.Message}");
            return null;
        }

        // tag 태그 가져오기
        XmlNodeList tagNodes = xmlDoc.SelectNodes($"/protocol/group/tag[@addr='{targetAddr}']");
        if (tagNodes == null || tagNodes.Count == 0)
        {
            //Debug.LogError($"{targetAddr} 값의 addr 속성을 가진 tag가 없습니다.");
            return null;
        }

        // 첫 번째로 찾은 tag 태그를 대상으로 함
        XmlNode targetTag = tagNodes[0];
        Dictionary<string, string> attributesDict = new Dictionary<string, string>();

        // 해당 태그의 모든 속성을 딕셔너리에 저장
        if (targetTag.Attributes != null)
        {
            foreach (XmlAttribute attr in targetTag.Attributes)
            {
                attributesDict[attr.Name] = attr.Value;
            }
        }

        // 저장된 속성들의 딕셔너리 반환
        return attributesDict;
    }

    public Dictionary<string, string> GetTagAttributesBySetGroup(string xml, string setGroupId)
    {
        if (string.IsNullOrEmpty(xml))
        {
            //Debug.LogError("XML 내용이 비어 있습니다.");
            return null;
        }

        XmlDocument xmlDoc = new XmlDocument();
        try
        {
            xmlDoc.LoadXml(xml);
        }
        catch (XmlException ex)
        {
            //Debug.LogError($"XML 파싱 중 오류 발생: {ex.Message}");
            return null;
        }

        XmlNodeList tagNodes = xmlDoc.SelectNodes($"/protocol/tag[@setgroup='{setGroupId}']");
        if (tagNodes == null || tagNodes.Count == 0)
        {
            //Debug.LogError($"{setGroupId} 값의 setgroup 속성을 가진 tag가 없습니다.");
            return null;
        }        

        Dictionary<string, string> attributesDict = new Dictionary<string, string>();
        int nullNameCount = 0;
        foreach (XmlNode tagNode in tagNodes)
        {
            if (tagNode.Attributes["name"].Value.ToString() != string.Empty)
            {
                string tagName = tagNode.Attributes["name"].Value;
                string tagAddr = tagNode.Attributes["addr"].Value;                
                attributesDict[tagName] = tagAddr;
            }
            else
            {
                nullNameCount++;
                string tagName = $"namenull_{nullNameCount}";
                string tagAddr = tagNode.Attributes["addr"].Value;                
                attributesDict[tagName] = tagAddr;
            }
        }
        return attributesDict;
    }
}