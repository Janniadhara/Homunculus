INCLUDE GlobalVariables.ink
~ finishStep("TalkToMother")
~ FollowLucy = "Completed"
~ finishQuest("FollowLucy")
Hello traveler. I have never seen your face bevore, you must be new in this small village. But how did you cross the canyon when the bridge is broken?
    * [Canyon, bridge? No I didn't cross them. I somehow found myself in a place you call the Slime Sancturay.]
    - Then how did you get there?
    * [I don't really know. Your daughter told me that I came out of a magical light.]
    - You don't know? Have you lost your memories?
    * [I think so. I can only remember my name, {playerName}.]
    - Hmm.. You said you came from a magical light. That's quite unusual. The Slime Sanctuary isn't known for magical occurrences. Perhabs some magic spell went wrong and in thats why you lost your memories.
    * [Well, sounds plausible to me.]
    - Regardless, you seem harmless enough. For the time being you can make yourself at home in the empty hous at the entrance of our village.
    * [Oh, thank you. I'll be sure to repay your kindness.]
    - Ah, and you should talk to (insert name here). They know a bit about magic and can probably help you solve the mystery of your appearence in the Slime Sanctuary. 
    * Ok. Thanks for your help
        ~ startQuest("test")