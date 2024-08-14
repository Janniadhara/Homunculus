INCLUDE GlobalVariables.ink
{FollowLucy == "notStarted" : -> WelcomePlayer | {FollowLucy == "inProgress" : -> FollowMeInProgress | {FollowLucy == "Completed" : -> FollowMeCompleted}}}

===WelcomePlayer===
Whoa! What just happened? You came out of that bright magic light! Are you a wizard? How did you do that?
    * [I don't think I am a wizard..]
    - Pity. I want to see magic just like in the stories of the Hero.
    * [Where am I?]
    - You are at the Slime Sanctuary.
    * [The Slime Sanctuary? What is that?]
    - That's how this place is called by the adults. My parents said, that the Slimes are peaceful and that it's save to play here.
    * [That sounds nice. By the way, what's your name?]
    - I'm Lucy. Lucy Venhana, and yours?
    * [Nice to meet you Lucy Venhana. My name is {playerName}]
    - What a cool name! Almost as cool as that magic you came from! You should come and meet my Mum. She's the village chief.
    * [Am I in trouble?]
    - What? No. She just always tells me "Lucy, if you meet a person you don't know, you have to come back to me with them". I think she wants to meet you.
    * [Okay, little girl. Let's meet your Mother]
    - Nice! Follow me, and don't fall back. And I'm not little anymore!
    * [I will. And sorry, of cause you are not little.]
    ~ FollowLucy = "inProgress"
    ~ startQuest("FollowLucy")
    -> END

===FollowMeInProgress===
You need to talk to my mom!
    *Okay
    -> END
    
===FollowMeCompleted===
I like to play with the Slimes!
    * I can see
    -> END