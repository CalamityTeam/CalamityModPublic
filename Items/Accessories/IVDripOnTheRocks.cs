using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace CalamityMod.Items.Accessories
{
    public enum AlcoholType
    {
        None,
        BaconOil,
        BloodyMary,
        CaribbeanRum,
        CinnamonRoll,
        Everclear,
        EvergreenGin,
        Fireball,
        GrapeBeer,
        Manhattan,
        Margarita,
        Moonshine,
        MoscowMule,
        OldFashioned,
        PurpleHaze,
        RedWine,
        Rum,
        Screwdriver,
        StarBeamRye,
        Tequila,
        TequilaSunrise,
        Vodka,
        Whiskey,
        WhiteWine,
        Ale,
        Sake,
    }

    public interface IAlcoholItem
    {
        Action<Player, float> IVDripAlcoholEffect { get; }
        AlcoholType AlcoholVariant { get; }
        LocalizedText DripEffectText { get; }
    }

    public class IVDripOnTheRocks : ModItem, ILocalizedModType
    {
        public int containedAlcoholID = -1;
        public AlcoholType currentAlcoholType = AlcoholType.None;
        public new string LocalizationCategory => "Items.Accessories";

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new Terraria.DataStructures.DrawAnimationVertical(1, 26));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.accessory = true;
        }

        public override void Load()
        {
            On_ItemSlot.OverrideLeftClick += IvDripAlcoholEquip;
        }

        public override void Unload()
        {
            On_ItemSlot.OverrideLeftClick -= IvDripAlcoholEquip;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["ContainedAlcoholID"] = containedAlcoholID;
            tag["CurrentAlcoholType"] = (int)currentAlcoholType;
        }

        public override void LoadData(TagCompound tag)
        {
            containedAlcoholID = tag.GetInt("ContainedAlcoholID");

            if (tag.ContainsKey("CurrentAlcoholType"))
                currentAlcoholType = (AlcoholType)tag.GetInt("CurrentAlcoholType");
            else
                currentAlcoholType = AlcoholType.None;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(containedAlcoholID);
            writer.Write((int)currentAlcoholType);
        }

        public override void NetReceive(BinaryReader reader)
        {
            containedAlcoholID = reader.ReadInt32();
            currentAlcoholType = (AlcoholType)reader.ReadInt32();
        }

        private bool IvDripAlcoholEquip(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot)
        {
            if (context == ItemSlot.Context.InventoryItem || context == ItemSlot.Context.ChestItem)
            {
                Item mouseItem = Main.mouseItem;
                Item targetItem = inv[slot];

                // Vanilla alcohols have to be applied manually
                bool isMouseAle = mouseItem.type == ItemID.Ale;
                bool isTargetAle = targetItem.type == ItemID.Ale;
                bool isMouseSake = mouseItem.type == ItemID.Sake;
                bool isTargetSake = targetItem.type == ItemID.Sake;

                // Alcohol into this acc
                if (targetItem.ModItem is IVDripOnTheRocks drip && (mouseItem.ModItem is IAlcoholItem || isMouseAle || isMouseSake))
                {
                    if (drip.containedAlcoholID == -1)
                    {
                        drip.containedAlcoholID = mouseItem.type;
                        drip.currentAlcoholType = isMouseAle ? AlcoholType.Ale : (isMouseSake ? AlcoholType.Sake : (mouseItem.ModItem as IAlcoholItem).AlcoholVariant);

                        mouseItem.stack--;
                        if (mouseItem.stack <= 0)
                            mouseItem.TurnToAir();

                        SoundEngine.PlaySound(SoundID.Grab);
                        return true;
                    }
                }

                if (mouseItem.ModItem is IVDripOnTheRocks dripHeld && (targetItem.ModItem is IAlcoholItem || isTargetAle || isTargetSake))
                {
                    if (dripHeld.containedAlcoholID == -1)
                    {
                        dripHeld.containedAlcoholID = targetItem.type;
                        dripHeld.currentAlcoholType = isTargetAle ? AlcoholType.Ale : (isTargetSake ? AlcoholType.Sake : (targetItem.ModItem as IAlcoholItem).AlcoholVariant);

                        targetItem.stack--;
                        if (targetItem.stack <= 0)
                            targetItem.TurnToAir();

                        SoundEngine.PlaySound(SoundID.Grab);
                        return true;
                    }
                }
            }
            return orig(inv, context, slot);
        }

        #region Shift-Right Click to Empty IV Drip

        // Allow shift-right clicking only if the IV drip actually contains alcohol
        public override bool CanRightClick() => containedAlcoholID != -1 && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

        public override void RightClick(Player player)
        {
            if (containedAlcoholID != -1)
            {
                // Give the alcohol back to the player
                player.QuickSpawnItem(player.GetSource_ItemUse(Item), containedAlcoholID);

                containedAlcoholID = -1;
                currentAlcoholType = AlcoholType.None;

                Item.stack++;

                SoundEngine.PlaySound(SoundID.Grab);
            }
        }
        #endregion
        public override void PreReforge()
        {
            if (containedAlcoholID != -1)
            {
                // Refund the alcohol to the player. 
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_ItemUse(Item), containedAlcoholID);

                containedAlcoholID = -1;
                currentAlcoholType = AlcoholType.None;
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            IVDripPlayer dripPlayer = player.GetModPlayer<IVDripPlayer>();
            dripPlayer.ivDripEquipped = true;

            if (containedAlcoholID != -1)
            {
                dripPlayer.currentAlcohol = currentAlcoholType;

                Item alcoholItem = new Item(containedAlcoholID);
                if (alcoholItem.ModItem is IAlcoholItem alcohol)
                    dripPlayer.ApplyIVDripAlcoholEffect(alcohol.IVDripAlcoholEffect);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (tooltips == null) return;

            var effectLine = tooltips.FirstOrDefault(x => x.Text.Contains("[EFFECT]"));
            var nameLine = tooltips.FirstOrDefault(x => x.Text.Contains("[NAME]"));

            if (containedAlcoholID == -1)
            {
                if (effectLine != null)
                {
                    effectLine.Text = this.GetLocalizedValue("Empty");
                }
                nameLine?.Hide();
                return;
            }

            if (effectLine != null)
            {
                if (containedAlcoholID == ItemID.Ale || containedAlcoholID == ItemID.Sake)
                    effectLine.Text = this.GetLocalizedValue("SakeAndAleText");

                else
                {
                    Item alcoholItem = new Item(containedAlcoholID);
                    if (alcoholItem.ModItem is IAlcoholItem alcohol)
                        effectLine.Text = alcohol.DripEffectText.Value;
                }
            }

            if (nameLine != null)
            {
                string alcoholName = Lang.GetItemNameValue(containedAlcoholID);
                nameLine.Text = this.GetLocalizedValue("CurrentAlcoholText") + $" [c/FF81E4:{alcoholName}]";
            }
        }

        // Draw the correct alcohol variant frame
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            int frameHeight = texture.Height / 26;
            int frameIndex = (int)currentAlcoholType;

            Rectangle targetFrame = new Rectangle(0, frameIndex * frameHeight, texture.Width, frameHeight);

            spriteBatch.Draw(texture, position, targetFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            int frameHeight = texture.Height / 26;
            int frameIndex = (int)currentAlcoholType;

            Rectangle targetFrame = new Rectangle(0, frameIndex * frameHeight, texture.Width, frameHeight);

            Vector2 drawPosition = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - frameHeight / 2);
            Vector2 origin = targetFrame.Size() / 2f;

            spriteBatch.Draw(texture, drawPosition, targetFrame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class IVDripPlayer : ModPlayer
    {
        public Dictionary<Action<Player, float>, float> IVDripAlcoholEffects = new();
        public bool ivDripEquipped;
        public AlcoholType currentAlcohol = AlcoholType.None;

        // would otherwise be spamming these checks manually any time another class wants to check for if IV drip has a certain alcohol filled
        public bool HasAlcohol(AlcoholType type)
        {
            return ivDripEquipped && currentAlcohol == type;
        }

        public override void ResetEffects()
        {
            IVDripAlcoholEffects.Clear();
            ivDripEquipped = false;
            currentAlcohol = AlcoholType.None;
        }

        public void ApplyIVDripAlcoholEffect(Action<Player, float> effect)
        {
            if (effect == null)
                return;

            if (IVDripAlcoholEffects.ContainsKey(effect))
                IVDripAlcoholEffects[effect] += 1f;
            else
                IVDripAlcoholEffects[effect] = 1f;
        }

        public override void PostUpdateEquips()
        {
            foreach (var effect in IVDripAlcoholEffects)
            {
                effect.Key.Invoke(Player, effect.Value);
            }
        }
    }
}
