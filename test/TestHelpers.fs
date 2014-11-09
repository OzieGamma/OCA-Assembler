(*

Copyright 2005-2009 Microsoft Corporation
Copyright 2013 Jack Pappas

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

COPIED AND EDITED By Oswald Maskens from
https://github.com/fsprojects/VisualFSharpPowerTools/blob/master/tests/FSharpVSPowerTools.Core.Tests/TestHelpers.fs
*)

/// Helper functions for implementing unit tests.
[<AutoOpen>]
module TestHelpers

open NUnit.Framework

(* Fluent test helpers for use with NUnit and FsUnit. *)
/// Asserts that two values are equal.
let inline shouldEq<'T when 'T : equality> (expected : 'T) (actual : 'T) =
    Assert.AreEqual (expected, actual)

/// Asserts that two values are NOT equal.
let inline shouldNotEq<'T when 'T : equality> (expected : 'T) (actual : 'T) =
    Assert.AreNotEqual (expected, actual)

/// Asserts that two objects are identical.
let inline shouldSame<'T when 'T : not struct> (expected : 'T) (actual : 'T) =
    Assert.AreSame (expected, actual)

/// Asserts that two objects are NOT identical.
let inline shouldNotSame<'T when 'T : not struct> (expected : 'T) (actual : 'T) =
    Assert.AreNotSame (expected, actual)

/// Asserts that a condition is true.
let inline shouldBeTrue condition =
    Assert.IsTrue (condition)

/// Asserts that a condition is false.
let inline shouldBeFalse condition =
    Assert.IsFalse (condition)


/// Assertion functions for collections.
[<RequireQualifiedAccess>]
module Collection =

    /// Asserts that two collections are exactly equal.
    /// The collections must have the same count, and contain the exact same objects in the same order.
    let inline shouldEq<'T, 'U when 'T :> seq<'U>> (expected : 'T) (actual : 'T) =
        CollectionAssert.AreEqual (expected, actual)

    /// Asserts that two collections are not exactly equal.
    let inline shouldNotEq<'T, 'U when 'T :> seq<'U>> (expected : 'T) (actual : 'T) =
        CollectionAssert.AreNotEqual (expected, actual)

    /// Asserts that two collections are exactly equal.
    /// The collections must have the same count, and contain the exact same objects but the match may be in any order.
    let inline shouldEquiv<'T, 'U when 'T :> seq<'U>> (expected : 'T) (actual : 'T) =
        try CollectionAssert.AreEquivalent (expected, actual) 
        with _ -> System.Diagnostics.Debug.WriteLine (sprintf "Expected: %A, actual: %A" expected actual); reraise()

    /// Asserts that two collections are not exactly equal.
    let inline shouldNotEquiv<'T, 'U when 'T :> seq<'U>> (expected : 'T) (actual : 'T) =
        CollectionAssert.AreNotEquivalent (expected, actual)


