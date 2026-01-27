using System.Security.Cryptography;
using System.Text;

namespace SecureJournal.Data.Services
{
    // PIN lock service for protecting the app before access
    public class PinLockService
    {
        // Preferences key for storing the hashed PIN
        private const string PrefPinHash = "securejournal.pin.hash";

        // Default PIN used only on first run (before user changes it)
        private const string DefaultPin = "123456";

        // Lock state used by PinGate to show/hide the overlay
        public bool IsUnlocked { get; private set; }

        // Event for UI refresh when lock state changes
        public event Action? LockStateChanged;

        public PinLockService()
        {
            EnsurePinHashExists();
            IsUnlocked = false;
        }

        // Validates entered PIN by comparing hashes
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

        // Locks the app again (used by a logout/lock button)
        public void Lock()
        {
            IsUnlocked = false;
            Notify();
        }

        // Sets a new 6-digit PIN (stored as a hash)
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

        // Ensures a PIN exists for first-time app runs
        private void EnsurePinHashExists()
        {
            var existing = Preferences.Get(PrefPinHash, "");
            if (!string.IsNullOrWhiteSpace(existing))
                return;

            Preferences.Set(PrefPinHash, ComputeHash(DefaultPin));
        }

        // Notifies listeners that lock state has changed
        private void Notify()
        {
            LockStateChanged?.Invoke();
        }

        // SHA256 hashing helper for storing PIN securely
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
