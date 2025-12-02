using System.Collections.Generic;

namespace MusicPlayer
{
    public class Playlist
    {
        public string Name { get; set; }
        public List<string> Songs { get; set; }

        public Playlist(string name)
        {
            Name = name;
            Songs = new List<string>();
        }

        public void AddSong(string path)
        {
            if (!Songs.Contains(path))
            {
                Songs.Add(path);
            }
        }
    }
}