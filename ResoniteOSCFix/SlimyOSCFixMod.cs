using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using Rug.Osc;

namespace SlimyOSCFix
{
	public class SlimyOSCFixMod : ResoniteMod
	{
		public override string Name => "OSCFixMod";
		public override string Author => "Slime-Senpai";
		public override string Version => "1.0.0";

		public override void OnEngineInit()
		{
			Harmony harmony = new Harmony("moe.sli.OSCFixMod");
			harmony.PatchAll();
			
		}

		[HarmonyPatch(typeof(OSC_Sender), "Start")]
		class OSC_Sender_Start_Patch
		{
			public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
			{
				var matcher = new CodeMatcher(instructions);

				var targetForZero = new[]
				{
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldloc_1),
					new CodeMatch(OpCodes.Ldarg_0)
				};

				matcher.MatchEndForward(targetForZero);

				var toInsertZero =
					new CodeInstruction(OpCodes.Ldc_I4_0);

				matcher.Insert(toInsertZero);

				var targetNew = new[]
				{
					new CodeMatch(OpCodes.Newobj)
				};

				matcher.MatchStartForward(targetNew);

				var toSetCtor = new CodeInstruction(OpCodes.Newobj,
					AccessTools.Constructor(typeof(OscSender), new[] { typeof(IPAddress), typeof(int), typeof(int) }));

				matcher.SetInstruction(toSetCtor);

				return matcher.InstructionEnumeration();
			}
		}
	}
}