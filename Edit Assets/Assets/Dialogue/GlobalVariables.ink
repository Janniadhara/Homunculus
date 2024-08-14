VAR test = "nope"
VAR talking = false
VAR playerName = "Player"
//quests
VAR Kill5Slimes = "notStarted"
VAR FollowLucy = "notStarted"

EXTERNAL startQuest(questId)
EXTERNAL finishStep(questId)
EXTERNAL finishQuest(questId)
EXTERNAL takeItemsFromPlayer(questId)