using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ Model ]
[System.Serializable]
public class AudioManagerModel
{
    public enum AUDIOTYPE // 오디오 파일의 타입
    {
        BG, //BackGround Music
        SE, //Sound Effect

        _MAX
    }

    [System.Serializable]
    public class AudioFile
    {
        [SerializeField] public string name;
        [SerializeField] public AUDIOTYPE type; // 재생할 오디오 파일의 타입
        [SerializeField] AudioClip clip = null;  // 재생할 오디오 파일
        public AudioClip Clip { get { return clip; } set { clip = value; name = value.name; } } //클립 변경 시 name도 같이 바뀌도록
        [SerializeField] public bool playOnAwake = false; // 씬 시작 시 재생.
        [SerializeField] public bool loop = false;  //루프 여부
        [SerializeField] public bool isOneShot = true;  //PlayOneShot 사용 여부
    }

    //인게임에 있는 AudioSource
    public AudioSource bg_source, se_source;
    //인게임 AudioSource에 현재 있는 클립 바로 수정 될 수 있도록 설정
    public AudioClip bg_clip { get { return bg_source.clip; } set { bg_source.clip = value; } }
    public AudioClip se_clip { get { return se_source.clip; } set { se_source.clip = value; } }

    [Header("사용되는 오디오 파일 리스트")]
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
    /// 오디오 파일 리스트에서 파일 가져오기
    /// </summary>
    /// <param name="_name">audioFile_lst에 등록된 오디오 파일</param>
    public AudioManagerModel.AudioFile GetAudioFile(string _name)
    {
        for (i = 0; i < model.audioFile_lst.Count; i++)
        {
            if (model.audioFile_lst[i].name == _name)
                return model.audioFile_lst[i];
        }
        if (_name != null)
            Debug.LogWarning("존재하지 않는 오디오 파일입니다.");
        return null;
    }

    /// <summary>
    /// 오디오 파일 리스트 재 정리(clip를 대입해주고 이름을 재설정)
    /// </summary>
    [ContextMenu("오디오 파일 리스트 재 정리")]
    public void RefreshAudioFileList()
    {
        for (i = 0; i < model.audioFile_lst.Count; i++)
        {
            // 이름 자동 설정
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
    /// 오디오 파일 재생
    /// </summary>
    /// <param name="_file">audioFile_lst에 등록된 오디오 파일</param>
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
                Debug.LogWarning("AUDIOTYPE이 없는 종류입니다.");
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
    /// 오디오 파일 정지
    /// </summary>
    /// <param name="_type">종료시킬 오디오 종류</param>
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
                Debug.LogWarning("AUDIOTYPE이 없는 종류입니다.");
                return;
        }

        tmp_source.Stop();
    }
}
