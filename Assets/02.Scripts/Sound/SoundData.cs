using System;
using System.Collections.Generic;
using UnityEngine;
using static SoundEnum;

[Serializable]
public struct BgmEntry
{
    public BgmType Type;
    public AudioClip Clip;
}

[Serializable]
public struct SfxEntry
{
    public SfxType Type;
    public AudioClip Clip;
}

[CreateAssetMenu(fileName = "SoundData", menuName = "Scriptable Objects/SoundData")]
public class SoundData : ScriptableObject
{
    [Header("BGM List")]
    public List<BgmEntry> BgmList = new List<BgmEntry>();

    [Header("SFX List")]
    public List<SfxEntry> SfxList = new List<SfxEntry>();
}