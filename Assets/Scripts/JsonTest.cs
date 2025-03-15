using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class Data
{
    public string Name;
    public float Height;
    [JsonProperty] // �� �Ӽ����� ���� private ����� Serailize �ȴ�
    private string secret;

    public Data(string name, float height, string secret)
    {
        Name = name;
        Height = height;
        this.secret = secret;
    }

    public override string ToString()
    {
        return Name + " " + Height + " " + secret;
    }
}

public class JsonTest : MonoBehaviour
{
    void Start()
    {
        // ��� �߰�
        Data charles = new Data("ö��", 198, "�߹ٴڿ� ���� �ΰ�");
        Data yongjun = new Data("����", 120, "����Դϴ�");

        // ����ȭ
        string json1 = JsonConvert.SerializeObject(charles);
        string json2 = JsonConvert.SerializeObject(yongjun);

        #region // ������ȭ �׽�Ʈ
        /*Data firstData = JsonConvert.DeserializeObject<Data>(json1);
        Debug.Log(firstData); // ������ȭ �ص� name, height�� ��� */
        #endregion

        Save(charles, "firstData.txt");
        Save(yongjun, "secondData.txt");
    }

    void Save<T>(T data, string fileName) // ���� ����
    {
        // ��� ����
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // ��� Ȯ��
        Debug.Log(path);

        // ����ó��
        try
        {
            // ����ȭ
            string json = JsonConvert.SerializeObject(data);

            // ��ȣȭ
            json = SimpleEncryptionUtility.Encrypt(json);

            // ���� ����
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    T Load<T>(string fileName) // ���� �ҷ�����
    {
        // �������
        string path = Path.Combine(Application.persistentDataPath, fileName); 

        try
        {
            // ������ �ִٸ�
            if (File.Exists(path)) 
            {
                // ������ �о����
                string json = File.ReadAllText(path);

                // ��ȣȭ
                json = SimpleEncryptionUtility.Decrypt(json);

                // ������ȭ�Ͽ� ������ �����͸� ����
                return JsonConvert.DeserializeObject<T>(json); 
            }
            else // ������ ���ٸ�
            {
                return default; //���׸����� null�� �ȵ����� �־ ������ ���´� �̷��� default�� ����
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return default;
        }
    }
}
