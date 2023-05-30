using Cysharp.Threading.Tasks;
using HarmonyLib;
using ngov3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace NSOMediaExtender
{
    [HarmonyPatch]
    public class AudioExtender
    {
        internal static List<Sound> customSoundList = new List<Sound>();
        static Sound currentSound;

        /// <summary>
        /// Plays a custom <c>Sound</c> from the Custom Sound List.
        /// </summary>
        /// <param name="Id">The <c>Sound</c> that you want to play, using their Id.</param>
        public static void PlayCustomSound(string Id)
        {
            Sound sound = customSoundList.FirstOrDefault(x => x.Id == Id); ;
            SetCurrentSound(Id, sound);
        }

        /// <summary>
        /// Creates a new <c>Sound</c> from an <c>AudioClip</c>, and adds it to the Custom Sound List.
        /// </summary>
        /// <param name="clip">The <c>AudioClip</c> used to make the <c>Sound.</c></param>
        public static void NewSoundFromClip(AudioClip clip)
        {

            Sound sound = CreateSound(clip.name, clip);
            if (sound != null)
            {
                customSoundList.Add(sound);
            };


        }

        /// <summary>
        ///  Creates a new <c>Sound</c> from an audio file, and adds it to the Custom Sound List.
        /// </summary>
        /// <remarks>Supported audio types include .mp3, .wav, .ogg, and more. </remarks>
        /// <param name="path">The path to the audio file.</param>
        /// <param name="type">The type of audio file.</param>
        /// <returns></returns>
        public static async UniTask NewSoundFromFile(string path, AudioType type)
        {
            Debug.Log(path);
            if (!File.Exists(path))
            {
                Debug.LogError("This path does not exist!");
                return;
            }
            string[] getFileName = path.Split('\\');
            string fileName = getFileName[getFileName.Count() - 1];

            try
            {
                using (var audioRequest = UnityWebRequestMultimedia.GetAudioClip(Path.Combine("file://" + path), type))
                {
                    audioRequest.SendWebRequest();
                    await UniTask.WaitUntil(() => audioRequest.isDone);
                    DownloadHandlerAudioClip dHandler = (DownloadHandlerAudioClip)audioRequest.downloadHandler;
                    dHandler.streamAudio = true;
                    await UniTask.WaitUntil(() => dHandler.isDone);
                    AudioClip audio = DownloadHandlerAudioClip.GetContent(audioRequest);
                    if (audio == null)
                    {
                        Debug.LogError("Could not load file!");
                        return;
                    }
                    Sound sound = CreateSound(fileName, audio);
                    if (sound != null)
                    {
                        customSoundList.Add(sound);
                    }
                }


            }
            catch (Exception ex)
            {
                Debug.LogError("An error has occurred: " + ex);
                return;
            }

        }

        static void SetCurrentSound(string soundName, Sound sound)
        {
            try
            {
                currentSound = sound;
                if (currentSound != null)
                {
                    if (currentSound.Id.StartsWith("BGM_"))
                    {
                        SingletonMonoBehaviour<AudioManager>.Instance.PlayBgmById(soundName);
                    }
                    if (currentSound.Id.StartsWith("SE_"))
                    {
                        SingletonMonoBehaviour<AudioManager>.Instance.PlaySeById(soundName);
                    }
                }
            }
            catch { }
        }
        internal static Sound CreateSound(string id, AudioClip clip)
        {

            Debug.LogWarning("Sound could not be found. Creating new sound...");
            Sound sound = new Sound
            {
                Id = id,
                Music = null,
                Clip = clip,
                VolumeType = VolumeType.REGULAR

            };
            if (sound.Id.StartsWith("BGM_"))
            {
                sound.Category = "BGM_REGULAR";
            }
            else if (sound.Id.StartsWith("SE_"))
            {
                sound.Category = "SE_REGULAR";
            }
            else
            {
                Debug.LogError("Could not create sound. AudioClip does not start with \"BGM_\" or \"SE_\"");
                return null;
            }
            return sound;


        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AudioManager), "GetCurrentSourceById")]
        static void ApplyCustomSound(ref AudioManager.TargetAudio __result, List<AudioSource> ____audioSources)
        {
            if (__result == null && currentSound != null)
            {
                string mixerName = currentSound.Category ?? "";
                AudioSource audioSource = ____audioSources.First((AudioSource x) => x.outputAudioMixerGroup.name == mixerName);
                __result = new AudioManager.TargetAudio
                {
                    Sound = currentSound,
                    Source = audioSource,
                    PlayTime = currentSound.Clip.length

                };
            }
            currentSound = null;
        }
    }
}
