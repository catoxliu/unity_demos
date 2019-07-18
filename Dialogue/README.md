## Simple demo for a Dialogue UI

parse the json file at build time in Editor and split all the dialog information
into patterns and store the results in ScriptableObject, which will be used directly
in runtime. 

Use menu "Dialogue/ParseJSON" will parse the JSON/dialogue.json file and create
some assets under Dialogues folder and then turn all that assets to dialogue_content
asset bundle.

Menu "Dialogue/BuildAB" could generate new asset bundles for current target platform.

Only Test in Android. But iOS should work as well.

Due to restriction, the texture and audio resource could not be uploaded.
