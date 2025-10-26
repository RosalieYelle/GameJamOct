-> start

=== start ===
#sprite:neutral Hi there! Welcome to our emotion test.
-> emotion_choice

=== emotion_choice ===
How am I feeling right now?

+ I'm happy!
    #sprite:happy Yay! I'm so happy you're here!
    -> emotion_choice

+ I'm angry!
    #sprite:angry Grrr! Why is everything SO ANNOYING!?
    -> emotion_choice

+ I'm surprised!
    #sprite:surprised Whoa!! I did not see that coming!
    -> emotion_choice

+ I'm done testing!
    #sprite:neutral Okay! Back to normal.
    -> END

