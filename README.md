# Scoreboard
## The task
This code was put together as an implementation of the following task:
> Live Football World Cup Scoreboard library that shows all the ongoing matches and their scores.
>
>**The scoreboard supports the following operations:**
>1. Start a new match, assuming initial score 0 – 0 and adding it the scoreboard   
>     This should capture following parameters:
>     - Home team
>     - Away team
>2. Update score. This should receive a pair of absolute scores: home team score and away team score.
>3. Finish match currently in progress. This removes a match from the scoreboard.
>4. Get a summary of matches in progress ordered by their total score. The matches with the same total score will be returned ordered by the most recently started match in the scoreboard.

  <br>
On top of these the following assumptions were made :  

- **Teams are uniquely identified by their name.**  
In real life that might not be the case. To differentiate it would be necessary for teams to have separate unique IDs. It was assumed out of scope of this task.
- **Teams cannot be playing more than one match at the same time**  
Though not speficied explicitly, it seems like a logical conclusion.
- **Negative score is not allowed at any time**  
This is a solution tailored for football (soccer) world cup. In this context only positive score is valid input. A solution encompassing all sports and cases could have that revisited. It would need a much more sophisticated desing than the one provided here.
- **Score can be changed from any value to any other value at any time**  
The actual matches seen goals disallowed after livefeed got updated, or human mistakes, where the teams and their scores were mixed up. For this reason it was assumed better to refrain from any protection against team's score going down.  
In real life scenario this should be discussed with domain experts if there aren't any more specific rules to this.  

## Tests  
The test were written using the "black box" technique, i.e. without intercepting, stubbing and asserting the state of the scoreboard's internal. The public API itself was used to setup the tests, perform the subjct action, and assert the final outcome. It means that tests complement each other, e.g. the happy path of saving a match score is not tested directly, but as setup before retrieving and evaluating the results.
From this perspective these are behavioural test, albeit not using the typical Given-When-Then syntax.  
With the current implementation these function as unit tests, where the scoreboard module is the unit. Replacing the persistence layer, represented by the repository, would turn them into integration test. This is easily achievable, as it only requires changing the `SetupScoreboard` function - no changes are required in the tests themselves.
Finally, these were 
They were written using the TDD approach, what hopeffuly can be seen in the history of this repository. Each commit is either a new test with minimalistic implementation, or a refactoring effort.

## Library/module design    
The intended public API of the module is defined by the `Scoreboard` class and the `MatchScore` DTO, deliberately defined in the same file. This is the only file that should be needed to understand how to communicate with the module and consume its output.  
The rest has been gradually migrated into its separate folders/namespaces. Were it possible in C#, then these would be forcefully hidden.  
Amonge these is the `MatchScoreModel` representing the write (persistence) model necessary to realise all requirements. Beside it is the interface and default, in-memory implementation of a `*MatchScoreRepository`. It provides the potentially valuable opportunity to replace the persistence layer.  
Finally, the `Domain` namespace contains the `Rules` class which holds named implementations of all business and validation rules that were identified along the way.
The benefits of the current design are as follows:  
- The rules can be easily modified or extended, immediately taking effect, as opposed to being scattered and repeated in many methods.
- It can be evolved towards Domain Driven Design, by introducing:
  - `Score` as a Value Object to accumulate and guard any current and future invariants.
  - `Match(Score)` as an Aggregate, taking over the encapsulation and guarding of most of the `Rules`.
- At the same time it can be evolved into a CQRS implementation, turning current `StartMatch`, `UpdateScore`, and `FinishMach` methods into commands and the `GetSummary` method into a query.
  - Interestingly, this would make the tests a bit more maintainable, allowing them to use the names of command parameters directly, as opposed to 'magically knowing' what the parameter name is.