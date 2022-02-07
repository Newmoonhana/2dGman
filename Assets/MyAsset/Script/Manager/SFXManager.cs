using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AUDIOTYPE // ����� ������ Ÿ��
{
    BG, //BackGround Music
    SE, //Sound Effect

    _MAX
}

[System.Serializable]
public class AudioFile
{
    [SerializeField] public string name;
    [SerializeField] public AUDIOTYPE type; // ����� ����� ������ Ÿ��
    [SerializeField] public AudioClip clip = null;  // ����� ����� ����
    public AudioClip Clip { get { return clip; } set { clip = value; name = value.name; } } //Ŭ�� ���� �� name�� ���� �ٲ��
    [SerializeField] public bool playOnAwake = false; // �� ���� �� ���.
    [SerializeField] public bool loop = false;  //���� ����
}

public class SFXManager : SingletonPattern_IsA_Mono<SFXManager>
{
    //tmp
    int i;
    AudioSource tmp_source;

    //�ΰ��ӿ� �ִ� AudioSource
    AudioSource bg_source, se_source;
    //�ΰ��� AudioSource�� ���� �ִ� Ŭ�� �ٷ� ���� �� �� �ֵ��� ����
    public AudioClip bg_clip { get { return bg_source.clip; } set { bg_source.clip = value; } }
    public AudioClip se_clip { get { return se_source.clip; } set { se_source.clip = value; } }

    [Header("���Ǵ� ����� ���� ����Ʈ")]
    [SerializeField]
    List<AudioFile> audioFile_lst;
    public AudioFile GetAudioFile(string _name)
    {
        for (i = 0; i < audioFile_lst.Count; i++)
        {
            if (audioFile_lst[i].name == _name)
                return audioFile_lst[i];
        }
        if (_name != null)
            Debug.LogWarning("�������� �ʴ� ����� �����Դϴ�.");
        return null;
    }
    [ContextMenu("����� ���� ����Ʈ �� ����")]
    void RefreshAudioFileList()
    {
        for (i = 0; i < audioFile_lst.Count; i++)
        {
            // �̸� �ڵ� ����
            audioFile_lst[i].Clip = audioFile_lst[i].clip;
        }
    }

    private void Awake()
    {
        if (!DontDestroyInst(this))
            return;

        RefreshAudioFileList();
        bg_source = transform.GetChild((int)AUDIOTYPE.BG).GetComponent<AudioSource>();
        se_source = transform.GetChild((int)AUDIOTYPE.SE).GetComponent<AudioSource>();
    }

    /// <summary>
    /// ����� ���� ���
    /// </summary>
    /// <param name="_file">SFXManager audioFile_lst�� ��ϵ� ����� ����</param>
    public void Play(AudioFile _file)
    {
        if (_file == null)
            return;

        switch (_file.type)
        {
            case AUDIOTYPE.BG:
                tmp_source = bg_source;
                break;
            case AUDIOTYPE.SE:
                tmp_source = se_source;
                break;
            default:
                Debug.LogWarning("AUDIOTYPE�� ���� �����Դϴ�.");
                return;
        }

        tmp_source.clip = _file.Clip;
        tmp_source.playOnAwake = _file.playOnAwake;
        tmp_source.loop = _file.loop;
        tmp_source.Play();
    }

    /// <summary>
    /// ����� ���� ����
    /// </summary>
    /// <param name="_type">�����ų ����� ����</param>
    public void Stop(AUDIOTYPE _type)
    {
        switch (_type)
        {
            case AUDIOTYPE.BG:
                tmp_source = bg_source;
                break;
            case AUDIOTYPE.SE:
                tmp_source = se_source;
                break;
            default:
                Debug.LogWarning("AUDIOTYPE�� ���� �����Դϴ�.");
                return;
        }

        tmp_source.Stop();
    }
}
