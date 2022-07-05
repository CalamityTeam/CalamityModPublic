using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Items.BaseItems
{
    /// <summary>
    /// A item that only exists as a held item. Useful if for example, it is used as a mean to temporarily replace the players attack ability.
    /// </summary>
    public abstract class HeldOnlyItem : ModItem
    {
        public override void Load()
        {
            On.Terraria.Player.dropItemCheck += DontDropCoolStuff;
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += LockMouseToSpecialItem;
        }
        public override void Unload()
        {
            On.Terraria.Player.dropItemCheck -= DontDropCoolStuff;
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int -= LockMouseToSpecialItem;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => false;
        
        public override void PostUpdate()
        {
            //Die if in the world
            Item.type = 0;
            Item.stack = 0;
        }

        public override bool CanPickup(Player player) => false;
        

        private void LockMouseToSpecialItem(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (!(Main.mouseItem.ModItem is HeldOnlyItem))
                orig(inv, context, slot);
        }

        //https://media.discordapp.net/attachments/458432092301295618/993675527539916850/unknown.png
        private void DontDropCoolStuff(On.Terraria.Player.orig_dropItemCheck orig, Terraria.Player self)
        {
            if (!(Main.mouseItem.ModItem is HeldOnlyItem))
                orig(self);
        }
    }
}
