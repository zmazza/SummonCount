using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SummonCount
{
	public class MinionCountDisplay : ModSystem
	{
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1)
			{
				layers.Insert(resourceBarIndex + 1, new LegacyGameInterfaceLayer(
					"SummonCount: Minion Counter",
					delegate {
						DrawMinionCount();
						return true;
					},
					InterfaceScaleType.UI));
			}
		}

		private void DrawMinionCount()
		{
			Player player = Main.LocalPlayer;

			// Check if player has a summoner weapon equipped
			if (!IsSummonerWeaponEquipped(player))
				return;

			float currentMinions = 0;
			int maxMinions = player.maxMinions;

			// Count active minions using minion slots
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.owner == player.whoAmI && proj.minion)
				{
					currentMinions += proj.minionSlots;
				}
			}

			// Format the text as "x/y"
			string minionText = $"{(int)currentMinions}/{maxMinions}";

			// Position to the right of the equipment bar (left side of screen)
			// Equipment slots are about 50 pixels wide each, positioned at top left
			Vector2 position = new Vector2(480, 30);

			// Draw the text
			Color textColor = currentMinions >= maxMinions ? Color.Red : Color.White;
			Utils.DrawBorderStringFourWay(
				Main.spriteBatch,
				Terraria.GameContent.FontAssets.MouseText.Value,
				minionText,
				position.X,
				position.Y,
				textColor,
				Color.Black,
				Vector2.Zero,
				1f
			);
		}

		private bool IsSummonerWeaponEquipped(Player player)
		{
			// Check if the held item is a summoner weapon
			Item heldItem = player.HeldItem;

			if (heldItem == null || heldItem.IsAir)
				return false;

			// Check if it's a summon weapon by checking damage class
			return heldItem.CountsAsClass(DamageClass.Summon);
		}
	}
}
