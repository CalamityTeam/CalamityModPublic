using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace CalamityMod.Graphics.Renderers.CalamityRenderers
{
    public class DyeableShadersRenderer : BaseRenderer
    {
        #region Fields/Properties
        public static Dictionary<IDyeableShaderRenderer, ManagedRenderTarget> Targets
        {
            get;
            private set;
        }

        public static Dictionary<IDyeableShaderRenderer, ArmorShaderData> Dyes
        {
            get;
            private set;
        }

        private static List<IDyeableShaderRenderer> RenderersToDrawThisFrame;

        public override DrawLayer Layer => DrawLayer.Player;

        // This ignores MainTarget, and handles its own targets, so always call the draw methods.
        public override bool ShouldDraw => true;
        #endregion

        #region Loading
        public override void Load()
        {
            Targets = [];
            Dyes = [];
            RenderersToDrawThisFrame = [];
        }

        public override void Unload()
        {
            Targets = null;
            Dyes = null;
            RenderersToDrawThisFrame = null;
        }
        #endregion

        #region Detour Stuff
        internal static void FindDyesDetour(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

            if (Main.dedServ)
                return;

            if (armorItem.ModItem is not IDyeableShaderRenderer drawer)
                return;

            // Store the dye in the slot.
            Dyes[drawer] = GameShaders.Armor.GetShaderFromItemId(dyeItem.type);
        }

        internal static void CheckVanityDetour(On_Player.orig_ApplyEquipVanity_Item orig, Player self, Item currentItem)
        {
            orig(self, currentItem);

            if (Main.dedServ)
                return;

            CheckIfEquipIsValid(currentItem, false);
        }

        internal static void CheckAccessoryDetour(On_Player.orig_ApplyEquipFunctional orig, Player self, Item currentItem, bool hideVisual)
        {
            orig(self, currentItem, hideVisual);

            if (Main.dedServ)
                return;

            CheckIfEquipIsValid(currentItem, hideVisual);
        }

        internal static void CheckArmorSetsDetour(On_Player.orig_UpdateArmorSets orig, Player self, int i)
        {
            orig(self, i);

            if (Main.dedServ)
                return;

            // Check each armor piece in the same manner as tMod.
            // If the entire set is equipped, and it is a renderer, it will be marked as valid.
            Item head = self.armor[0];
            Item body = self.armor[1];
            Item legs = self.armor[2];

            if (head.ModItem != null && head.ModItem.IsArmorSet(head, body, legs) && head.ModItem is IDyeableShaderRenderer renderer)
                MarkAsValid(renderer);


            if (body.ModItem != null && body.ModItem.IsArmorSet(head, body, legs) && body.ModItem is IDyeableShaderRenderer renderer2)
                MarkAsValid(renderer2);


            if (legs.ModItem != null && legs.ModItem.IsArmorSet(head, body, legs) && legs.ModItem is IDyeableShaderRenderer renderer3)
                MarkAsValid(renderer3);
        }

        private static void CheckIfEquipIsValid(Item item, bool hideVisual)
        {
            if (Main.dedServ)
                return;

            // Difficulty mode checks.
            if ((item.expertOnly && !Main.expertMode) || (item.masterOnly && !Main.masterMode))
                return;

            // Item exists, is visible and is a mod item checks.
            if (item.IsAir || hideVisual || item.ModItem == null)
                return;

            // Item uses the interface check.
            if (item.ModItem is not IDyeableShaderRenderer dyeableShaderRenderer)
                return;

            MarkAsValid(dyeableShaderRenderer);
        }

        private static void MarkAsValid(IDyeableShaderRenderer renderer)
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                // If it doesn't have a dictonary entry, create one.
                if (!Targets.ContainsKey(renderer))
                    Main.QueueMainThreadAction(() => Targets[renderer] = new(true, ManagedRenderTarget.CreateScreenSizedTarget));
            });

            // Mark this item as drawable this frame.
            RenderersToDrawThisFrame.AddWithCondition(renderer, renderer.ShouldDrawDyeableShader);
        }
        #endregion

        #region Updates/Drawing
        // Clear the list at the beginning of each update, to ensure its only populated by correct ones.
        public override void PreUpdate() => RenderersToDrawThisFrame?.Clear();

        public override void DrawToTarget(SpriteBatch spriteBatch)
        {
            if (Main.dedServ)
                return;

            // Leave if nothing to draw.
            if (!RenderersToDrawThisFrame.Any())
                return;

            // Sort the list by draw order.
            RenderersToDrawThisFrame = RenderersToDrawThisFrame.OrderByDescending(renderer => renderer.RenderDepth).ToList();

            foreach (var renderer in RenderersToDrawThisFrame)
            {
                if (!Targets.TryGetValue(renderer, out var target))
                    continue;

                // Swap to the assosiated target and call the interface method.
                target.SwapTo();
                renderer.DrawDyeableShader(spriteBatch);
            }
        }

        public override void DrawTarget(SpriteBatch spriteBatch)
        {
            if (Main.dedServ)
                return;

            // Leave if nothing to draw.
            if (!RenderersToDrawThisFrame.Any())
                return;

            foreach (var renderer in RenderersToDrawThisFrame)
            {
                if (!Targets.TryGetValue(renderer, out var target))
                    continue;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

                // If it is dyeable, and a dye exists, apply it. This has null safety, as dyeShader can be null here.
                if (renderer.ShaderIsDyeable && Dyes.TryGetValue(renderer, out var dyeShader))
                    dyeShader?.Apply(null, new(target, Vector2.Zero, new Rectangle(0, 0, target.Width, target.Height), Color.White));

                // Draw the assosiated target that has been drawn to.
                spriteBatch.Draw(target, Vector2.Zero, Color.White with { A = 0 });
            }
        }
        #endregion
    }
}
