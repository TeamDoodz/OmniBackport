using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Encounters;
using InscryptionAPI.Guid;
using OmniBackport.Abilities;
using UnityEngine;

namespace OmniBackport.Nodes.Gemify {
	public class GemifySequencer : CardStatBoostSequencer, ICustomNodeSequence {
		public static readonly MechanicsConcept GemifyMechanic = GuidManager.GetEnumValue<MechanicsConcept>(MainPlugin.GUID, nameof(GemifyMechanic));

		public IEnumerator ExecuteCustomSequence(CustomNodeData nodeData) {
			selectionSlot = SpecialNodeHandler.Instance.cardStatBoostSequencer.selectionSlot;
			confirmStone = SpecialNodeHandler.Instance.cardStatBoostSequencer.confirmStone;
			pile = SpecialNodeHandler.Instance.cardStatBoostSequencer.pile;

			selectionSlot.specificRenderers[0].material.mainTexture = MainPlugin.assets.LoadPNG("card_slot_gemify");

			ViewManager.Instance.SwitchToView(View.Default, false, true);
			yield return new WaitForSeconds(0.5f);

			Singleton<TableRuleBook>.Instance.SetOnBoard(true);

			InteractionCursor.Instance.SetEnabled(true);

			// This clears the fire animation component from the slot.
			if(selectionSlot.specificRenderers.Count > 1) {
				selectionSlot.specificRenderers.ForEach((x) => x.enabled = false);
			}

			if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(GemifyMechanic)) {
				yield return TextDisplayer.Instance.ShowUntilInput("You witnessed a huge, extravagant building in the distance and decided to investigate.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput("After a short walk, you gazed in awe at what you had discovered...", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput($"Standing tall in front of you, in its astonishing glory, was one of the {"Gemifiers".Gemify()} of ancient legend.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return new WaitForSeconds(0.5f);
			}

			selectionSlot.gameObject.SetActive(true);
			selectionSlot.RevealAndEnable();
			selectionSlot.ClearDelegates();
			selectionSlot.CursorSelectStarted = (Action<MainInputInteractable>)Delegate.Combine(selectionSlot.CursorSelectStarted, new Action<MainInputInteractable>(OnSlotSelected));

			if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(GemifyMechanic)) {
				yield return new WaitForSeconds(0.5f);

				yield return TextDisplayer.Instance.ShowUntilInput("As if expecting your arrival, several [c:bR]refugee mages[c:] welcomed you into the complex.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput("\"[c:bG]As a reward for the troubles you must have gone through in order to have found us...[c:]\", one explained.", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
				yield return TextDisplayer.Instance.ShowUntilInput($"\"[c:bG]We shall allow one of your [c:][c:bR]students[c:][c:bG] to enter our [c:]{"gemifier".Gemify()}[c:bR].[c:]\"", letterAnimation: TextDisplayer.LetterAnimation.WavyJitter);
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
				yield return TextDisplayer.Instance.ShowUntilInput("Unfortunately, you bore no cards fitting for this action.");
				yield return TextDisplayer.Instance.ShowUntilInput("You cut your losses and left the scene.");
				yield return CleanUpAndExit();
			} else {
				confirmStone.Exit();
				selectionSlot.SetEnabled(false);
				selectionSlot.Card.SetInteractionEnabled(false);
				selectionSlot.Card.Anim.PlayFaceDownAnimation(true, false);
				AudioController.Instance.PlaySound2D("totem_activate", MixerGroup.TableObjectsSFX, 1f, 0f, null, null, null, null, false);
				yield return new WaitForSeconds(1f);
				RunState.Run.playerDeck.ModifyCard(selectionSlot.Card.Info, GetGemifyMod());
				selectionSlot.Card.SetInfo(selectionSlot.Card.Info);
				selectionSlot.Card.Anim.PlayFaceDownAnimation(false, false);

				yield return new WaitForSeconds(0.1f);
				ViewManager.Instance.SwitchToView(View.StatBoostSlot, false, true);
				yield return new WaitForSeconds(0.5f);

				if(!SaveManager.SaveFile.progressionData.learnedMechanics.Contains(GemifyMechanic)) {
					yield return TextDisplayer.Instance.ShowUntilInput($"The mages had empowered your [c:bR]{selectionSlot.Card.Info.DisplayedNameLocalized}[c:] with a unique ability.");
					yield return TextDisplayer.Instance.ShowUntilInput($"It can now draw from the power of your {"gems".Gemify()}.");
					yield return TextDisplayer.Instance.ShowUntilInput($"You thanked the mages and continued on with your journey.");
					yield return new WaitForSeconds(0.5f);
				}

				SaveManager.SaveFile.progressionData.learnedMechanics.Add(GemifyMechanic);

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
			if(selectionSlot.specificRenderers.Count > 1) {
				selectionSlot.specificRenderers.ForEach((x) => x.enabled = true);
			}

			yield return new WaitForSeconds(0.1f);
			GameFlowManager.Instance.TransitionToGameState(GameState.Map);
		}

		public static CardModificationInfo GetGemifyMod() => new CardModificationInfo() {
			abilities = new List<Ability>() { Gemified.ability },
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
			return card.Abilities.Contains(Gemified.ability);
		}
	}
}
