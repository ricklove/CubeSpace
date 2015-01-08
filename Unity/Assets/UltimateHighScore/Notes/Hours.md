Ultimate High Score

# Description

Unity Asset Store Asset

Create the best looking high score GUI ever!
Effects that will make your players come back for more!

## Components

- Score Text
- Score Target
- Combo Bar (like DDR - no mistakes makes it grow, long chains makes it glow)
	- Combo ball: Coins with springs between them in a springy physics ball
- Score with "Coins" at Position
	- "+ 10"
	- 10 "Coins" fly from position to score
	- Particle effects (sparkles)
- Combo at Position
	- "x2" bubble text popup/fadeout under coins message
- Register multiple score elements

## Score Element

Multiple score elements

- ID
- Text Type (Int, Time)
- TextPosition
- CoinPrefab
- IncludeCombos
- ComboBarPosition
- ComboTimeout (min time to continue combo)
- Font
- BaseFontSize
- ScoreMessageScale
- ComboMessageScale
- CoinScale


## Logic

- Score.Instance.AddScore("Score", 1000);
- Score.Instance.ResetCombo("Score");
- Score.Instance.AddScore("Time", 3);
- Score.Instance.RemoveScore("Time", 5); 


# Hours

## Hour 1

### 2015-01-08 4:15-4:37

- Create folder in CubeSpace project

### 4:38-4:57

- Consider design:
	- Make it fun!
	- Include a toy? (Chain Reaction maybe?)

- Consider how to include a toy...

### 5:07-5:52

- Layout UI

## Hour 2

### 5:53-

- Adjust Score



## TODO

- Make it FUN!
- Worry about functionality first 

- Add graphical assets
