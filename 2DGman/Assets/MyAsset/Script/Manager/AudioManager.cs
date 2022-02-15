using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ Model ]
[System.Serializable]
public class AudioManagerModel
{
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
        [SerializeField] AudioClip clip = null;  // ����� ����� ����
        public AudioClip Clip { get { return clip; } set { clip = value; name = value.name; } } //Ŭ�� ���� �� name�� ���� �ٲ��
        [SerializeField] public bool playOnAwake = false; // �� ���� �� ���.
        [SerializeField] public bool loop = false;  //���� ����
        [SerializeField] public bool isOneShot = true;  //PlayOneShot ��� ����
    }

    //�ΰ��ӿ� �ִ� AudioSource
    public AudioSource bg_source, se_source;
    //�ΰ��� AudioSource�� ���� �ִ� Ŭ�� �ٷ� ���� �� �� �ֵ��� ����
    public AudioClip bg_clip { get { return bg_source.clip; } set { bg_source.clip = value; } }
    public AudioClip se_clip { get { return se_source.clip; } set { se_source.clip = value; } }

    [Header("���Ǵ� ����� ���� ����Ʈ")]
    [SerializeField]
    public List<AudioFile> audioFile_lst;
}

// [ Controller ]
public class AudioManager : SingletonPattern_IsA_Mono<AudioManager>
{
    public AudioManagerModel model;
    //tmp
    int i;
    AudioSource tmp_source;

    /// <summary>
    /// ����� ���� ����Ʈ���� ���� ��������
    /// </summary>
    /// <param name="_name">audioFile_lst�� ��ϵ� ����� ����</param>
    public AudioManagerModel.AudioFile GetAudioFile(string _name)
    {
        for (i = 0; i < model.audioFile_lst.Count; i++)
        {
            if (model.audioFile_lst[i].name == _name)
                return model.audioFile_lst[i];
        }
        if (_name != null)
            Debug.LogWarning("�������� �ʴ� ����� �����Դϴ�.");
        return null;
    }

    /// <summary>
    /// ����� ���� ����Ʈ �� ����(clip�� �������ְ� �̸��� �缳��)
    /// </summary>
    [ContextMenu("����� ���� ����Ʈ �� ����")]
    public void RefreshAudioFileList()
    {
        for (i = 0; i < model.audioFile_lst.Count; i++)
        {
            // �̸� �ڵ� ����
            model.audioFile_lst[i].Clip = model.audioFile_lst[i].Clip;
        }
    }

    private void Awake()
    {
        if (!DontDestroyInst(this))
            return;

        RefreshAudioFileList();
        model.bg_source = transform.GetChild((int)AudioManagerModel.AUDIOTYPE.BG).GetComponent<AudioSource>();
        model.se_source = transform.GetChild((int)AudioManagerModel.AUDIOTYPE.SE).GetComponent<AudioSource>();
    }

    /// <summary>
    /// ����� ���� ���
    /// </summary>
    /// <param name="_file">audioFile_lst�� ��ϵ� ����� ����</param>
    public void Play(string _filename)
    {
        AudioManagerModel.AudioFile _file = GetAudioFile(_filename);
        if (_file == null)
            return;

        switch (_file.type)
        {
            case AudioManagerModel.AUDIOTYPE.BG:
                tmp_source = model.bg_source;
                break;
            case AudioManagerModel.AUDIOTYPE.SE:
                tmp_source = model.se_source;
                break;
            default:
                Debug.LogWarning("AUDIOTYPE�� ���� �����Դϴ�.");
                return;
        }

        if (_file.isOneShot)
        {
            tmp_source.PlayOneShot(_file.Clip);
        }
        else
        {
            if (_file.Clip == model.se_clip)
                if (tmp_source.isPlaying)
                    return;
            tmp_source.clip = _file.Clip;
            tmp_source.playOnAwake = _file.playOnAwake;
            tmp_source.loop = _file.loop;
            tmp_source.Play();
        }
    }

    /// <summary>
    /// ����� ���� ����
    /// </summary>
    /// <param name="_type">�����ų ����� ����</param>
    public void Stop(AudioManagerModel.AUDIOTYPE _type)
    {
        switch (_type)
        {
            case AudioManagerModel.AUDIOTYPE.BG:
                tmp_source = model.bg_source;
                break;
            case AudioManagerModel.AUDIOTYPE.SE:
                tmp_source = model.se_source;
                break;
            default:
                Debug.LogWarning("AUDIOTYPE�� ���� �����Դϴ�.");
                return;
        }

        tmp_source.Stop();
    }
}
