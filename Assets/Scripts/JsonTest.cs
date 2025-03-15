using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class Data
{
    public string Name;
    public float Height;
    [JsonProperty] // 이 속성으로 인해 private 멤버도 Serailize 된다
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
        // 멤버 추가
        Data charles = new Data("철수", 198, "발바닥에 점이 두개");
        Data yongjun = new Data("용준", 120, "비밀입니다");

        // 직렬화
        string json1 = JsonConvert.SerializeObject(charles);
        string json2 = JsonConvert.SerializeObject(yongjun);

        #region // 역직렬화 테스트
        /*Data firstData = JsonConvert.DeserializeObject<Data>(json1);
        Debug.Log(firstData); // 역직렬화 해도 name, height만 출력 */
        #endregion

        Save(charles, "firstData.txt");
        Save(yongjun, "secondData.txt");
    }

    void Save<T>(T data, string fileName) // 파일 저장
    {
        // 경로 지정
        string path = Path.Combine(Application.persistentDataPath, fileName);

        // 경로 확인
        Debug.Log(path);

        // 예외처리
        try
        {
            // 직렬화
            string json = JsonConvert.SerializeObject(data);

            // 암호화
            json = SimpleEncryptionUtility.Encrypt(json);

            // 파일 저장
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    T Load<T>(string fileName) // 파일 불러오기
    {
        // 경로지정
        string path = Path.Combine(Application.persistentDataPath, fileName); 

        try
        {
            // 파일이 있다면
            if (File.Exists(path)) 
            {
                // 파일을 읽어오고
                string json = File.ReadAllText(path);

                // 복호화
                json = SimpleEncryptionUtility.Decrypt(json);

                // 역직렬화하여 원래의 데이터를 형성
                return JsonConvert.DeserializeObject<T>(json); 
            }
            else // 파일이 없다면
            {
                return default; //제네릭으로 null이 안들어갈수도 있어서 에러가 나온다 이럴땐 default로 변경
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return default;
        }
    }
}
