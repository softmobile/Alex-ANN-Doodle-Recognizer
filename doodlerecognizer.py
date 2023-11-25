import tensorflow as tf
import numpy as np
import json
import time

categories = ["anvil", "book", "door", "hat", "lollipop", "rake", "apple", "megaphone", "moon", "pants", "saw", "star", "tree", "umbrella", "wheel"]

location = "C:/Users/dyrek/OneDrive/Documents/Code Projects/Doodle Neural Network/"
drawingPath = location + "drawing.json"

modelPath = location + "doodlebrain.model"
model = tf.keras.models.load_model(modelPath)

prevDrawing = None
while True:
    try:
        with open(drawingPath, 'r') as file:
            data = json.load(file)
    except (json.decoder.JSONDecodeError, PermissionError):
        time.sleep(0.05)
        continue

    npDrawing = np.array(data.get("drawing"))
    npDrawing = npDrawing.reshape(1, 28, 28)
    npDrawing = npDrawing / 255.0

    if np.array_equal(prevDrawing, npDrawing) or np.max(npDrawing) < 1:
        continue

    prevDrawing = npDrawing

    predictions = model.predict(npDrawing)

    sortedIndices = np.argsort(predictions.flatten())[::-1]
    sortedPredictions = predictions.flatten()[sortedIndices]
    sortedCategories = np.array(categories)[sortedIndices]

    sortedPList = sortedPredictions.tolist()

    resultsDict = {}
    for i, prediction in enumerate(sortedPList):
        resultsDict[sortedCategories[i]] = prediction

    try:
        with open(location + "results.json", 'w') as results:
            json.dump(resultsDict, results)
    except PermissionError:
        time.sleep(0.05)
        continue

    time.sleep(0.05)