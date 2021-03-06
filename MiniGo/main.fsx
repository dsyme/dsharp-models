// Copyright 2019 The TensorFlow Authors, adapted by the DiffSharp authors. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#r @"..\bin\Debug\netcoreapp3.1\publish\DiffSharp.Core.dll"
#r @"..\bin\Debug\netcoreapp3.1\publish\DiffSharp.Backends.ShapeChecking.dll"
#r @"..\bin\Debug\netcoreapp3.1\publish\Library.dll"

open MiniGo

open DiffSharp

let boardSize = 19
let simulationCountForOneMove = 40

let gameConfiguration = GameConfiguration(
    size: boardSize,
    komi: 7.5,
    isVerboseDebuggingEnabled: false)

// Creates the GoModel and loads the checkpoint.
print("Loading checkpoint into GoModel. Might take a while.")
let modelConfig = ModelConfiguration(boardSize: boardSize)
let model = GoModel(configuration: modelConfig)

let remoteCheckpoint = Uri(
    string: "https://storage.googleapis.com/s4tf-hosted-binaries/checkpoints/MiniGo/000939-heron")!
let reader = try MiniGoCheckpointReader(checkpointLocation: remoteCheckpoint, modelName: "MiniGo")
model.load(reader)

let predictor = MCTSModelBasedPredictor(boardSize: boardSize, model: model)

// Pick up policies to play. The first policy in `participants` plays black.
// Current available policies are:
//   - RandomPolicy
//   - HumanPolicy
//   - MCTSPolicy
let mctsConfiguration = MCTSConfiguration(
    gameConfiguration: gameConfiguration,
    simulationCountForOneMove: simulationCountForOneMove)

try playOneGame(
    gameConfiguration: gameConfiguration,
    participants: [
        MCTSPolicy(
            participantName: "black", predictor: predictor,
            configuration: mctsConfiguration),
        MCTSPolicy(
            participantName: "white", predictor: predictor,
            configuration: mctsConfiguration),
    ])
