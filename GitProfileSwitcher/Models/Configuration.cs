using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Foundation;

namespace GitProfileSwitcher.Models
{
    public class Configuration : IEquatable<Configuration>
    {
        private static readonly string _appSupportDir = NSSearchPath.GetDirectories(
                NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User)
                .FirstOrDefault();
        private const string ConfigurationFileName = "config.json";

        public List<Profile> Profiles { get; set; } = new List<Profile>();
        public int CurrentProfileIndex { get; set; } = -1;
        public bool UseGravatar { get; set; } = false;

        public static async Task<Configuration> Load()
        {
            var configDirectory = Path.Combine(
                _appSupportDir, NSBundle.MainBundle.BundleIdentifier);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            string configFilePath = Path.Combine(configDirectory, ConfigurationFileName);
            if (File.Exists(configFilePath))
            {
                using var fileStream = new FileStream(configFilePath, FileMode.Open,
                    FileAccess.Read);
                return await JsonSerializer.DeserializeAsync<Configuration>(fileStream);
            }

            return new Configuration();
        }

        public async Task Save()
        {
            var configDirectory = Path.Combine(
                _appSupportDir, NSBundle.MainBundle.BundleIdentifier);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
            string configFilePath = Path.Combine(configDirectory, ConfigurationFileName);
            using var fileStream = new FileStream(configFilePath, FileMode.OpenOrCreate,
                FileAccess.Write);
            fileStream.SetLength(0);
            await fileStream.FlushAsync();
            fileStream.Position = 0;
            await JsonSerializer.SerializeAsync(fileStream, this);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Configuration);
        }

        public bool Equals(Configuration other)
        {
            return other != null &&
                   EqualityComparer<List<Profile>>.Default.Equals(Profiles, other.Profiles) &&
                   CurrentProfileIndex == other.CurrentProfileIndex &&
                   UseGravatar == other.UseGravatar;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Profiles, CurrentProfileIndex, UseGravatar);
        }
    }

    public class Profile : IEquatable<Profile>
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Profile);
        }

        public bool Equals(Profile other)
        {
            return other != null &&
                   Name == other.Name &&
                   Email == other.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Email);
        }
    }
}
