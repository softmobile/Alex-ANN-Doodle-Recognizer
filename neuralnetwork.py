import tensorflow as tf
import numpy as np

categories = ["anvil", "book", "door", "hat", "lollipop", "rake", "apple", "megaphone", "moon", "pants", "saw", "star", "tree", "umbrella", "wheel"]
x_train = []

for cat in categories:
    data = np.load("dataset/" + cat + ".npy")[:40000, :]
    x_train.append(data)

x_train = np.concatenate(x_train, axis=0)

category_mapping = {cat: i for i, cat in enumerate(categories)}
y_train = np.array([category_mapping[cat] for cat in categories for _ in range(40000)])

indices = np.arange(len(x_train))
np.random.shuffle(indices)
x_train = x_train[indices]
y_train = y_train[indices]

x_train = x_train.reshape(-1, 28, 28)

x_train = x_train / 255.0

print(x_train.shape)
print(y_train.shape)

model = tf.keras.models.Sequential()
model.add(tf.keras.layers.Flatten(input_shape=(28, 28)))
model.add(tf.keras.layers.Dense(128, activation='relu'))
model.add(tf.keras.layers.Dense(64, activation='relu'))
model.add(tf.keras.layers.Dense(len(categories), activation='softmax'))

optimizer = tf.keras.optimizers.Adam(learning_rate=0.002)
model.compile(optimizer=optimizer, loss='sparse_categorical_crossentropy', metrics=['accuracy'])

earlyStopping = tf.keras.callbacks.EarlyStopping(monitor='loss', patience=3)

model.fit(x_train, y_train, epochs=12, batch_size=32, callbacks=[earlyStopping])

model.save('doodlebrain.model')