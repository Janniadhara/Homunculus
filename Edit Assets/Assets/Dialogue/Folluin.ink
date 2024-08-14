INCLUDE GlobalVariables.ink

{
    - Kill5Slimes == "notStarted": -> QuestNotStarted
    - Kill5Slimes == "inProgress": -> QuestInProgress
    - Kill5Slimes == "canFinish": -> QuestFinished
    - else: -> JustHello
}
    
=== JustHello ===
Hello
    *Hi -> END
    
=== QuestNotStarted ===
Hello, can you get me 5 Slime Cores? I'll reward you handsomely.
    * Sure -> StartQuest
    //~ startQuest("Kill5Slimes")
    * No, sorry -> END

=== QuestInProgress ===
Hello, how is my quest going?
    * Good. I'm almost done -> END
    * Ah yes.. the quest.. don't worry, I'm on it -> END
    * I killed all the slimes. Just like you wanted.
    But as I see you don't have their Cores with you. Come back when you have them.
        ** Oh, sorry I'll get them. -> END
    
=== QuestFinished ===
Oh I see you have finished my quest.
    * Yes, here are the SLime Cores. -> FinishQuest

=== StartQuest ===
    ~ Kill5Slimes = "inProgress"
    ~ startQuest("Kill5Slimes")
    -> END
    
=== FinishQuest ===
    ~ Kill5Slimes = "QuestFinished"
    ~ takeItemsFromPlayer("Kill5Slimes")
    -> END