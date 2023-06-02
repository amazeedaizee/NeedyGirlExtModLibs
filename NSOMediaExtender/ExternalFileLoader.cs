using Cysharp.Threading.Tasks;
using ngov3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace NSOMediaExtender
{
    internal class ExternalFileLoader
    {
        /// <summary>
        /// The exception that is thrown if an error occured in loading the requested file.
        /// </summary>
        public class FileLoadFailedException : Exception
        {
            public FileLoadFailedException(string message, Exception inner) : base(message, inner) { }
            public FileLoadFailedException(string message) : base(message) { }
            public FileLoadFailedException() { }
        }

        /// <summary>
        ///  Creates a new <c>Sound</c> from an audio file, and adds it to the Custom Sound List.
        /// </summary>
        /// <remarks>Supported audio types include .mp3, .wav, .ogg, and more. </remarks>
        /// <param name="path">The path to the audio file.</param>
        /// <param name="type">The type of audio file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileLoadFailedException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <returns></returns>
        public static async UniTask<AudioClip> LoadAudioFromFile(string path, AudioType type)
        {
            if (path == null)
            {
                throw new ArgumentNullException("File path cannot be null.");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file cannot be found! From searching in {path}.");
            }
            string[] getFileName = path.Split('\\');
            string fileName = getFileName[getFileName.Count() - 1];
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
                        throw new FileLoadFailedException("Audio File failed to load.");
                    }
                    return audio;
                }
            

        }

        /// <summary>
        /// Loads a custom image file as a Sprite, and adds it to the <c>Sprite</c> Extlist. 
        /// </summary>
        /// <remarks>Supported image types are jpg, and png.</remarks>
        /// <param name="path">The path to the image file.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileLoadFailedException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static Sprite LoadImageFromFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("File path cannot be null.");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file cannot be found! From searching in {path}.");
            }
            string[] getFileName = path.Split('\\');
            string fileName = getFileName[getFileName.Count() - 1];
            byte[] file = File.ReadAllBytes(path);
            if
            (
                //JPEG
                ((file[0] == 0xFF && file[1] == 0xD8) && (file[file.LongLength - 2] == 0xFF && file[file.LongLength - 1] == 0xD9))
                //PNG
                || (file[0] == 0x89 && file[1] == 0x50 && file[2] == 0x4E && file[3] == 0x47 && file[4] == 0x0D && file[5] == 0x0A && file[6] == 0x1A && file[7] == 0x0A)

            // checking an image file by bytes first came from this StackOverflow post: https://stackoverflow.com/questions/670546/determine-if-file-is-an-image

            )
            {
                Texture2D tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, file, false);
                tex.filterMode = FilterMode.Point;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                sprite.name = fileName;
                if (sprite == null)
                {
                    throw new FileLoadFailedException("Image File failed to load.");
                }
                return sprite;
            }
            throw new ArgumentOutOfRangeException("File type is not jpeg or png.");
        }
    }
}
