using System;
using System.Collections.Generic;

[Serializable]
public class RemoteIntro
{
    public string type;       // "video", "image_text", "none"
    public string url;        // для video
    public string image_url;  // для image_text
    public string text;       // для image_text
    public string audio_url;  // опционально, любой тип
}

[Serializable]
public class RemoteMedia
{
    public string type; // "video", "image", "none"
    public string url;
}

[Serializable]
public class RemoteGirlData
{
    public int            id;
    public RemoteIntro    intro;
    public List<string>   photos; // URL на 1.jpg...7.jpg
    public RemoteMedia    win;
    public RemoteMedia    lose;
}

[Serializable]
public class RemoteGirlsResponse
{
    public List<RemoteGirlData> girls;
}
