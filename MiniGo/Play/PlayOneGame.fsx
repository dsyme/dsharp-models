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

/// Plays one game with participants. The game ends with two passes.
let playOneGame(gameConfiguration: GameConfiguration, participants: Policy[]) =
    let boardState = BoardState(gameConfiguration: gameConfiguration)
    Debug.Assert(participants.count = 2, "Must provide two participants.")
    Debug.Assert(participants[0].participantName <>  participants[1].participantName,
                 "Participants' names should not be same.")
    // Choose a random participant to play black.
    let shuffled = participants.shuffled()
    let blackPlayer = shuffled[0]
    let whitePlayer = shuffled[1]

    let previousMove: Move?
    let consecutivePassCount = 0

    // Loops until we get a winner or tie.
    while true do
        print(boardState)

        if gameConfiguration.isVerboseDebuggingEnabled then
            print($"Legal moves: {boardState.legalMoves.count}")
            print($"Stones on board: {boardState.stoneCount}")
            if let ko = boardState.ko then
                print($"Found ko: {ko}.")
            else
                print("No ko.")

        // Check whether the game ends with two passes.
        if consecutivePassCount >= 2 then
            print("End of Game. Score for black player: \(boardState.score(.black)).")
            break


        let policy: Policy
        match boardState.nextPlayerColor with
        | .black ->
            policy = blackPlayer
            print("-> Black")
        | .white ->
            policy = whitePlayer
            print("-> White")


        let move = policy.nextMove(boardState, after: previousMove)
        previousMove = move

        match move with
        | .pass ->
            consecutivePassCount <- consecutivePassCount + 1
            print("- Pass")
            boardState = boardState.passing()
        | .place(let position):
            consecutivePassCount = 0
            print($"- Placing stone at: {position}")
            boardState = try boardState.placingNewStone(at: position)



