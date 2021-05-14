# OpenSkiFree

An open source implementation of SkiFree in Unity

![](demo.gif)

# Train ML Agent

1. Open terminal and navigate to the project's Assets folder
2. `mlagents-learn config/skifree_config.yaml --run-id=newid1`

### Handy arguments

- By default mlagents-learn runs at 20x normal speed
- You can override this default using the --time-scale arg:
  - `mlagents-learn config/skifree_config.yaml --run-id=newid1 --time-scale 1`
