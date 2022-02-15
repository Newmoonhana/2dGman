using UnityEngine;
//출처: https://mrhook.co.kr/266 [리프(리뷰하는 프로그래머) TV] (해당 예제에서 원리적으로 안맞는 몇몇 부분을 수정해 사용).
public class SingletonPattern<T> where T : class, new()
{
    private static T m_instance = null;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new T();
            }
            return m_instance;
        }
    }
}
