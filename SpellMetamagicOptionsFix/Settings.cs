using UnityModManagerNet;

namespace SpellMetamagicOptionsFix
{
    public class Settings : UnityModManager.ModSettings
    {
        public static UnityModManager.ModEntry ModEntry;
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
