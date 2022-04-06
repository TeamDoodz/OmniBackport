using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiskCardGame;
using InscryptionAPI.Card;
using InscryptionAPI.Encounters;
using InscryptionAPI.Guid;
using UnityEngine;

namespace OmniBackport.Nodes.Overclock {
	public class OverclockSequencer : CardStatBoostSequencer, ICustomNodeSequence {
		public static readonly MechanicsConcept OverclockMechanic = GuidManager.GetEnumValue<MechanicsConcept>(MainPlugin.GUID,nameof(OverclockMechanic));

		public IEnumerator ExecuteCustomSequence(CustomNodeData nodeData) {
			selectionSlot = SpecialNodeHandler.Instance.cardStatBoostSequencer.selectionSlot;
			confirmStone = SpecialNodeHandler.Instance.cardStatBoostSequencer.confirmStone;
			pile = SpecialNodeHandler.Instance.cardStatBoostSequencer.pile;

			selectionSlot.specificRenderers[0].material.mainTexture = MainPlugin.assets.LoadPNG("card_slot_overclock");

			ViewManager.Instance.SwitchToView(View.Default, false, true);
			yield return new WaitForSeconds(0.5f);


			Singleton<TableRuleBook>.Instance.SetOnBoard(true);

			InteractionCursor.Instance.SetEnabled(true);

			// This clears the fire animation component from the slot.
			if(selectionSlot.specificRenderers.Count > 1) {
				selectionSlot.specificRenderers.ForEach((x) => x.enabled = false);
			}

			if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(OverclockMechanic)) {
				yield return TextDisplayer.Instance.ShowUntilInput("You stumbled across a derelict [c:bR]military factory[c:], well-hidden by the forestry and rubble.");
			}

			yield return new WaitForSeconds(0.5f);

			selectionSlot.gameObject.SetActive(true);
			selectionSlot.RevealAndEnable();
			selectionSlot.ClearDelegates();
			selectionSlot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(selectionSlot.CursorSelectStarted,new Action<MainInputInteractable>(OnSlotSelected));

			if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(OverclockMechanic)) {
				yield return new WaitForSeconds(0.5f);

				yield return TextDisplayer.Instance.ShowUntilInput("After investigating the site, you found that a part of the assembly line was still intact,", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput("But had only enough power for one more procedure.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput("You would have to choose the specimen for this operation carefully.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
			}

			Singleton<ViewManager>.Instance.SwitchToView(View.StatBoostSlot, false, false);

			yield return new WaitForSeconds(0.2f);

			yield return confirmStone.WaitUntilConfirmation();
			yield return ApplyOverclockedSequence(true);
		}

		private IEnumerator ApplyOverclockedSequence(bool isValidCard) {
			yield return new WaitForSeconds(0.1f);
			ViewManager.Instance.SwitchToView(View.Default, false, true);
			yield return new WaitForSeconds(0.5f);
			if(!isValidCard) {
				yield return TextDisplayer.Instance.ShowUntilInput("Unfortunately, you bore no cards fitting for this action.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput("You cut your losses and left the scene.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return CleanUpAndExit();
			} else {
				confirmStone.Exit();
				selectionSlot.SetEnabled(false);
				selectionSlot.Card.SetInteractionEnabled(false);
				selectionSlot.Card.Anim.PlayFaceDownAnimation(true, false);
				AudioController.Instance.PlaySound2D("teslacoil_charge", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
				yield return new WaitForSeconds(4f);
				AudioController.Instance.PlaySound2D("teslacoil_overload", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
				yield return new WaitForSeconds(0.5f);
				RunState.Run.playerDeck.ModifyCard(selectionSlot.Card.Info, GetOverclockMod());
				MainPlugin.logger.LogInfo($"Attack of selected card is now {selectionSlot.Card.Info.Attack}");
				selectionSlot.Card.SetInfo(selectionSlot.Card.Info);
				selectionSlot.Card.Anim.PlayFaceDownAnimation(false, false);

				yield return new WaitForSeconds(0.1f);
				ViewManager.Instance.SwitchToView(View.StatBoostSlot, false, true);
				yield return new WaitForSeconds(0.5f);
				if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(OverclockMechanic)) {
					yield return TextDisplayer.Instance.ShowUntilInput($"The ancient machinery had increased the [c:bR]power[c:] of your [c:bR]{selectionSlot.Card.Info.DisplayedNameLocalized}[c:]...", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
					yield return TextDisplayer.Instance.ShowUntilInput($"...but in the process made it incredibly [c:bR]fragile[c:].", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
					yield return TextDisplayer.Instance.ShowUntilInput($"You will need to be incredibly careful with it if you want it to survive.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
					yield return new WaitForSeconds(0.5f);
				}
				selectionSlot.SetEnabled(true);
				selectionSlot.ClearDelegates();
				selectionSlot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(selectionSlot.CursorSelectStarted, new Action<MainInputInteractable>(TakeCardAndFinishSequence));
			}
		}

		private IEnumerator CleanUpAndExit() {
			yield return new WaitForSeconds(0.5f);
			ViewManager.Instance.SwitchToView(View.Default, false, false);
			selectionSlot.SetEnabled(false);
			selectionSlot.SetShown(false, false);
			selectionSlot.gameObject.SetActive(false);
			yield return pile.DestroyCards();
			confirmStone.SetShown(false, false);

			yield return new WaitForSeconds(0.1f);
			GameFlowManager.Instance.TransitionToGameState(GameState.Map);
		}

		public static CardModificationInfo GetOverclockMod() => new CardModificationInfo() { 
			abilities = new List<Ability>() { Ability.PermaDeath },
			attackAdjustment = 2,
			fromCardMerge = true
		};

		private void TakeCardAndFinishSequence(MainInputInteractable _) {
			selectionSlot.FlyOffCard();
			StartCoroutine(CleanUpAndExit());
		}

		private void OnSlotSelected(MainInputInteractable slot) {
			List<CardInfo> validCards = GetValidCards();

			selectionSlot.SetEnabled(false);
			selectionSlot.ShowState(HighlightedInteractable.State.NonInteractable, false, 0.15f);

			if(validCards == null) {
				StartCoroutine(ApplyOverclockedSequence(false));
			} else if(validCards != null) {
				(slot as SelectCardFromDeckSlot).SelectFromCards(validCards, new Action(OnSlotSelectionEnded), false);
			}
		}

		private void OnSlotSelectionEnded() {
			selectionSlot.SetShown(true, false);
			selectionSlot.ShowState(HighlightedInteractable.State.Interactable, false, 0.15f);
			Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, true);
			if(selectionSlot.Card != null) {
				confirmStone.Enter();
			} else {
				confirmStone.Exit();
			}
		}

		private List<CardInfo> GetValidCards() {
			List<CardInfo> validCards = new List<CardInfo>();
			validCards.AddRange(RunState.Run.playerDeck.Cards);
			validCards.RemoveAll(IsCardInvalid);
			return validCards;
		}

		private bool IsCardInvalid(CardInfo card) {
			return StatIconInfo.IconAppliesToAttack(card.specialStatIcon) || card.Abilities.Contains(Ability.PermaDeath);
		}
	}
}
