// Copyright 2020 The TensorFlow Authors, adapted by the DiffSharp authors. All Rights Reserved.
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

open DiffSharp

/// Replay buffer to store the agent's experiences.
///
/// Vanilla Q-learning only trains on the latest experience. Deep Q-network uses
/// a technique called "experience replay", where all experience is stored into
/// a replay buffer. By storing experience, the agent can reuse the experiences
/// and also train in batches. For more information, check Human-level control
/// through deep reinforcement learning (Mnih et al., 2015).
type ReplayBuffer {
  /// The maximum size of the replay buffer. When the replay buffer is full,
  /// new elements replace the oldest element in the replay buffer.
  let capacity: int
  /// If enabled, uses Combined Experience Replay (CER) sampling instead of the
  /// uniform random sampling in the original DQN paper. Original DQN samples
  /// batch uniformly randomly in the replay buffer. CER always includes the
  /// most recent element and samples the rest of the batch uniformly randomly.
  /// This makes the agent more robust to different replay buffer capacities.
  /// For more information about Combined Experience Replay, check A Deeper Look
  /// at Experience Replay (Zhang and Sutton, 2017).
  let combined: bool

  /// The states that the agent observed.
  let states: [Tensor] = []
  /// The actions that the agent took.
  let actions: Tensor[] = []
  /// The rewards that the agent received from the environment after taking
  /// an action.
  let rewards: [Tensor] = []
  /// The next states that the agent received from the environment after taking
  /// an action.
  let nextStates: [Tensor] = []
  /// The episode-terminal flag that the agent received after taking an action.
  let isDones: [Tensor<Bool>] = []
  /// The current size of the replay buffer.
  let count: int { return states.count

  init(capacity: int, combined: bool) = 
    self.capacity = capacity
    self.combined = combined


  let append(
    state: Tensor,
    action: Tensor (*<int32>*),
    reward: Tensor,
    nextState: Tensor,
    isDone: Tensor<Bool>
  ) = 
    if count >= capacity then
      // Erase oldest SARS if the replay buffer is full
      states.removeFirst()
      actions.removeFirst()
      rewards.removeFirst()
      nextStates.removeFirst()
      isDones.removeFirst()

    states.append(state)
    actions.append(action)
    rewards.append(reward)
    nextStates.append(nextState)
    isDones.append(isDone)


  let sample(batchSize: int) = (
    stateBatch: Tensor,
    actionBatch: Tensor (*<int32>*),
    rewardBatch: Tensor,
    nextStateBatch: Tensor,
    isDoneBatch: Tensor<Bool>
  ) = 
    let indices: Tensor (*<int32>*)
    if self.combined = true then
      // Combined Experience Replay
      let sampledIndices = (0..<batchSize - 1).map { _ in int32.random(in: 0..<int32(count))
      indices = Tensor (*<int32>*)(shape=[batchSize], scalars: sampledIndices + [int32(count) - 1])
    else
      // Vanilla Experience Replay
      let sampledIndices = (0..batchSize-1).map { _ in int32.random(in: 0..<int32(count))
      indices = Tensor (*<int32>*)(shape=[batchSize], scalars: sampledIndices)


    let stateBatch = dsharp.tensor(stacking: states).gathering(atIndices: indices, alongAxis: 0)
    let actionBatch = dsharp.tensor(stacking: actions).gathering(atIndices: indices, alongAxis: 0)
    let rewardBatch = dsharp.tensor(stacking: rewards).gathering(atIndices: indices, alongAxis: 0)
    let nextStateBatch = dsharp.tensor(stacking: nextStates).gathering(atIndices: indices, alongAxis: 0)
    let isDoneBatch = dsharp.tensor(stacking: isDones).gathering(atIndices: indices, alongAxis: 0)

    (stateBatch, actionBatch, rewardBatch, nextStateBatch, isDoneBatch)


