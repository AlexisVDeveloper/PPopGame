using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityEngine;

namespace GameSaveLoad {
    public class IOSaveLoad<T>
    {
        private const string PATH_DIRECTORY = "/Saves/";
        private const string PATH_IMAGES_DIRECTORY = "/Saves/Images/";
        private const string EXTENSION_DATA = ".sav";
        private const string EXTENSION_IMAGE = ".png";
        public static void SaveData(T data, string name) {
            var path = Application.persistentDataPath + PATH_DIRECTORY;
            if(!File.Exists(path)) Directory.CreateDirectory (path);
            path = path + name + EXTENSION_DATA;

            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

            try {
                bf.Serialize(stream, data);
            } catch {
                Debug.LogError("Error to save data");
            } finally {
                stream.Close();
            }
        }

        public static void SaveImageData(Texture2D texture, string name) {
            var path = Application.persistentDataPath + PATH_IMAGES_DIRECTORY;
            if(!File.Exists(path)) Directory.CreateDirectory (path);
            path = path + name + EXTENSION_IMAGE;
            Debug.Log($"PATH: {path}");
            try {
                File.WriteAllBytes(path, texture.EncodeToPNG());
            } catch {
                Debug.LogError("Error to save Image");
            }
        }

        public static T LoadData(string name) {
            var path = Application.persistentDataPath + PATH_DIRECTORY + name + EXTENSION_DATA;
            if(File.Exists(path)) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);
                
                try {
                    T data = (T)bf.Deserialize(stream);
                    stream.Close();
                    return data;
                } catch {
                    Debug.LogError("Error to load data");
                } 
            }

            return default(T);
        }

        public static Texture2D LoadImage(string name) {
            var path = Application.persistentDataPath + PATH_IMAGES_DIRECTORY + name + EXTENSION_IMAGE;

            if(File.Exists(path)) {
                try {
                    byte[] bytesPNG = File.ReadAllBytes(path);
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(bytesPNG);
                    return texture;
                } catch {
                    Debug.LogError("Error to load Image");
                } 
            }

            return null;
        }
    }


}
