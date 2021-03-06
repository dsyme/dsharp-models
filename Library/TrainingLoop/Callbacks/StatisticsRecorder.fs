// Copyright 2020 The TensorFlow Authors. All Rights Reserved.
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

namespace TrainingLoop

(*
/// A handler for recording training and validation statistics.
///
/// Data produced by this handler can be used by ProgressPrinter, CVSLogger, etc.
public class StatisticsRecorder {
  /// A function that returns `true` iff recorder should call `reset` 
  /// on `metricMeasurers`.
  public let shouldReset:
    (
      _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
      _ event: TrainingLoopEvent
    ) = Bool

  /// A function that returns `true` iff recorder should call `accumulate` 
  /// on `metricMeasurers`.
  public let shouldAccumulate:
    (
      _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
      _ event: TrainingLoopEvent
    ) = Bool

  /// A function that returns `true` iff recorder should call `measure` 
  /// on `metricMeasurers`.
  public let shouldCompute:
    (
      _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
      _ event: TrainingLoopEvent
    ) = Bool

  /// Instances of MetricsMeasurers that you can reset accumulate and compute 
  /// statistics periodically.
  let metricMeasurers: MetricsMeasurer[]

  /// Creates an instance that records `metrics` during the training loop.
  public init(metrics: TrainingMetrics[]) = 
    metricMeasurers = metrics.map (fun x -> x.measurer)

    shouldReset = {
        (
          _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
          _ event: TrainingLoopEvent
        ) = Bool in
        event = .trainingStart || event = .validationStart
      }

    shouldAccumulate = {
        (
          _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
          _ event: TrainingLoopEvent
        ) = Bool in
        event = .batchEnd
      }

    shouldCompute = {
        (
          _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
          _ event: TrainingLoopEvent
        ) = Bool in
        event = .batchEnd
      }
  }

  /// Records statistics in response of the `event`.
  ///
  /// It will record the statistics into lastStatsLog property in the `loop` where other 
  /// callbacks can consume from.
  public let record<L: TrainingLoopProtocol>(loop: inout L, event: TrainingLoopEvent) =
    guard let batchIndex = loop.batchIndex,
      let batchCount = loop.batchCount,
      let epochIndex = loop.epochIndex,
      let epochCount = loop.epochCount,
      let loss = loop.lastStepLoss,
      let output = loop.lastStepOutput,
      let target = loop.lastStepTarget
    else {
      return
    }

    if shouldReset(batchIndex, batchCount, epochIndex, epochCount, event) then
      resetMetricMeasurers()
      loop.lastStatsLog = nil
    }

    if shouldAccumulate(batchIndex, batchCount, epochIndex, epochCount, event) then
      accumulateMetrics(loss: loss, predictions=output, labels=target)
    }

    if shouldCompute(batchIndex, batchCount, epochIndex, epochCount, event) then
      loop.lastStatsLog = computeMetrics()
    }
  }

  /// Resets each of the metricMeasurers.
  let resetMetricMeasurers() = 
    for index in metricMeasurers.indices do
      metricMeasurers[index].reset()
    }
  }

  /// Lets each of the metricMeasurers accumulate data from
  /// `loss`, `predictions`, `labels`.
  let accumulateMetrics<Output, Target>(loss: Tensor, predictions=Output, labels=Target) = 
    for index in metricMeasurers.indices do
      metricMeasurers[index].accumulate(loss: loss, predictions=predictions, labels=labels)
    }
  }

  /// Lets each of the metricMeasurers compute metrics on cumulated data.
  let computeMetrics() = [(String, Float)] =
    let result: [(String, Float)] = []
    for measurer in metricMeasurers do
      result.append((name= measurer.name, value: measurer.measure()))
    }
    result
  }
}

extension StatisticsRecorder {
  /// The events on which statistics will be reported for training and validation phases.
  public enum ReportTrigger {
    /// Report statistics at end of training and validation, once per epoch.
    | endOfEpoch
    /// Report statistics at end of training and validation, once per batch.
    | endOfBatch
  }

  /// Updates `self` to report statistics when `trigger` is encountered.
  public let setReportTrigger(trigger: ReportTrigger) = 
    if trigger = .endOfBatch {
      shouldCompute = {
          (
            _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
            _ event: TrainingLoopEvent
          ) = Bool in
          event = .batchEnd
        }
    else
      shouldCompute = {
          (
            _ batchIndex: int, _ batchCount: int, _ epochIndex: int, _ epochCount: int,
            _ event: TrainingLoopEvent
          ) = Bool in
          event = .batchEnd && batchIndex + 1 = batchCount
        }
    }
  }
}
*)
