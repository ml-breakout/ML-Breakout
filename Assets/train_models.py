"""
This script trains the models for the 3 AI difficulties and saves them to the agent-models directory.

# Detailed description

The script runs the mlagents-learn command to train the models. It then listens to the output of the 
process to determine when the models have reached the "medium" and "hard" training stages. At these stages, 
the script copies the most recent snapshot of the model and saves it as the "easy" and "medium" models 
respectively. When training has completed, it saves the last model snapshot as the "hard" model.

# How to run this

1. Start your mlagentsconda environment `conda activate mlagents` (see the README for more info)
2. Start this script `python3 train_models.py`  
3. When the script outputs `[INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.` 
   Go to the "18PlayerAITrainer" scene in Unity, and press play.
4. (wait for the script to finish)

When the script is done, it should have created new model snapshot (.onnx) files in the 
Assets/agent-models directory. These files are the trained models for the 3 AI difficulties and will
automatically be loaded by the Unity game the next time you run or built it.

"""

import subprocess
import re
import shutil

logging_prefix = "[Train Models] "

def log(message):
    print(logging_prefix + message)

def main():
    # Start the process
    command = "mlagents-learn agent-training-configs/config_v0.yaml --run-id=new_model --train --force"
    process = subprocess.Popen(
        command, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, shell=True
    )

    # Compile the regex patterns
    snapshot_regex = re.compile(r"Exported (\S+)")
    medium_stage_regex = re.compile("Parameter 'stage' is in lesson 'MediumAIStage'")
    hard_stage_regex = re.compile("Parameter 'stage' is in lesson 'HardAIStage'")

    most_recent_snapshot = None

    # Process each line of output from the process
    while True:
        output = process.stdout.readline()
        if output == "" and process.poll() is not None:
            break
        if output:
            print(output.rstrip())  # Print the output in real-time

            # Check a snapshot was just created
            snapshot_match = snapshot_regex.search(output)
            if snapshot_match:
                # Save the snapshot's path
                most_recent_snapshot = snapshot_match.group(
                    1
                )
                continue

            # If we've made it to the "medium" taining stage, copy the most recent snapshot and call it the "easy" model
            medium_stage_match = medium_stage_regex.search(output)
            if medium_stage_match:
                shutil.copy2(most_recent_snapshot, "./agent-models/easy.onnx")
                log("./agent-models/easy.onnx created!")
                continue

            # If we've made it to the "hard" taining stage, copy the most recent snapshot and call it the "medium" model
            hard_stage_match = hard_stage_regex.search(output)
            if hard_stage_match:
                shutil.copy2(most_recent_snapshot, "./agent-models/medium.onnx")
                log("./agent-models/medium.onnx created!")
                continue

            
    # Ensure the process has completed
    process.wait()

    # Copy the most recent snapshot and call it the "hard" model
    shutil.copy2(most_recent_snapshot, "./agent-models/hard.onnx")
    log("./agent-models/hard.onnx created!")

if __name__ == "__main__":
    main()
