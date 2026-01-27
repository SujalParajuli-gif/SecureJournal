using System.Security.Cryptography;
using System.Text;

namespace SecureJournal.Data.Services
{
    // App-wide PIN lock service (rewritten for SecureJournal)
    public class PinLockService
    {
        private const string PrefPinHash = "securejournal.pin.hash";
        private const string DefaultPin = "123456";

        public bool IsUnlocked { get; private set; }

        public event Action? LockStateChanged;

        public PinLockService()
        {
            EnsurePinHashExists();
            IsUnlocked = false;
        }

        public bool TryUnlock(string? enteredPin)
        {
            enteredPin = (enteredPin ?? "").Trim();

            if (enteredPin.Length == 0)
                return false;

            var savedHash = Preferences.Get(PrefPinHash, "");
            var enteredHash = ComputeHash(enteredPin);

            if (string.Equals(savedHash, enteredHash, StringComparison.Ordinal))
            {
                IsUnlocked = true;
                Notify();
                return true;
            }

            return false;
        }

        public void Lock()
        {
            IsUnlocked = false;
            Notify();
        }

        public bool SetNewPin(string? newPin)
        {
            newPin = (newPin ?? "").Trim();

            if (newPin.Length != 6)
                return false;

            if (!newPin.All(char.IsDigit))
                return false;

            Preferences.Set(PrefPinHash, ComputeHash(newPin));
            IsUnlocked = false;
            Notify();
            return true;
        }

        private void EnsurePinHashExists()
        {
            var existing = Preferences.Get(PrefPinHash, "");
            if (!string.IsNullOrWhiteSpace(existing))
                return;

            Preferences.Set(PrefPinHash, ComputeHash(DefaultPin));
        }

        private void Notify()
        {
            LockStateChanged?.Invoke();
        }

        private static string ComputeHash(string value)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(value ?? "");
            var hash = sha.ComputeHash(bytes);

            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));

            return sb.ToString();
        }
    }
}
