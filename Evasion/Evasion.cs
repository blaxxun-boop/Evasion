using System.Reflection;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using SkillManager;

namespace Evasion;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInIncompatibility("org.bepinex.plugins.valheim_plus")]
public class Evasion : BaseUnityPlugin
{
	private const string ModName = "Evasion";
	private const string ModVersion = "1.0.3";
	private const string ModGUID = "org.bepinex.plugins.evasion";

	public void Awake()
	{
		Skill evasion = new("Evasion", "evasion-icon.png");
		evasion.Description.English("Reduces the stamina usage of dodging.");
		evasion.Name.German("Ausweichen");
		evasion.Description.German("Reduziert die benötigte Ausdauer um auszuweichen.");
		evasion.Name.Chinese("闪避");
		evasion.Description.Chinese("减少闪避的耐力消耗");
		evasion.Configurable = true;

		Assembly assembly = Assembly.GetExecutingAssembly();
		Harmony harmony = new(ModGUID);
		harmony.PatchAll(assembly);
	}

	[HarmonyPatch(typeof(Player), nameof(Player.UpdateDodge))]
	private class ReduceStaminaUsage
	{
		[UsedImplicitly]
		private static void Prefix(Player __instance)
		{
			__instance.m_dodgeStaminaUsage *= 1 - __instance.GetSkillFactor("Evasion") * 0.5f;
		}

		[UsedImplicitly]
		private static void Postfix(Player __instance)
		{
			__instance.m_dodgeStaminaUsage /= 1 - __instance.GetSkillFactor("Evasion") * 0.5f;

			if (__instance.InDodge() && __instance.m_queuedDodgeTimer == 0)
			{
				__instance.RaiseSkill("Evasion");
			}
		}
	}
}
