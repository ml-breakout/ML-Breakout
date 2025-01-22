## How to train a new model

(Do this once) First, you'll need to install the mlagents python library:
1. Install conda: https://docs.conda.io/projects/conda/en/latest/user-guide/install/index.html. Conda allows you to manage multiple python versions and Python virtual environments (note: not virtual machines) on a single machine.
2. Create a virtual environment for mlagents, then activate it. Run, `conda create -n mlagents python=3.10.12 && conda activate mlagents` If you restart your terminal, you’ll need to re-run the `conda activate mlagents` command.
(Make sure to run the rest of these commands in that same terminal session, where the conda environment is activated)
3. (windows only) install pytorch into your virtual env: pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
4. Install the mlagents python package: python -m pip install mlagents==1.1.0

Whenever you want to train a new model:

1. Start the conda environment: `conda activate mlagents`
2. Enter the Assets directory: `cd Assets/`
3. Run `mlagents-learn agent-training-configs/config_v0.yaml --run-id=my_run_id --train` Change `my_run_id` to a name for your model. That script should run, then get to a point where it says it's waiting for Unity.
4. In Unity, open up the `SinglePlayerAITrainer` scene and press Play
5. (At this point, the ML Agents script should take control of the game and start playing through a bunch of training episodes. This can take about 30 minutes to run, so go grab lunch or something)

When that's finished, there should be some new files in the Assets/results/ directory for your model. In the next step, we'll tell the game to use that new model.

7. Open up the `BreakoutGameBoard` prefab.
8. Open the inspector for the `paddle` game object
9. Under `Behavior Parameters`, change the Model field to the model file you just created.

Great! Now your model will be used in all instances of BreakoutGameBoard that are controlled by an AI player.

To test out your new model, the easiest way is:

1. Open up the `SinglePlayerAI` scene and press Play.


## Debugging

Here are some handy tips for solving common problems:

* If the Unity editor crashes while you're trying to play a scene that includes an agent running in inference mode, check that the model you're using was trained using the same Vector Observation Stack Size as what's currently being used in your code. If there's a mismatch, that'll likely result in a crash (based on my experience)
