If you would like to contribute to DbUp, look for [up-for-grabs](https://github.com/DbUp/DbUp/labels/up-for-grabs) issues, post a comment that you are working on it then submit a pull request.

Contributions to the docs and reporting of issues is also welcome.

## Testing
Because of the range of databases DbUp supports, it is not feasible to do automated integration testing. Instead DbUp takes a different approach:

* Use RecordingDbConnection
  - RecordingDbConnection keeps a log of every opened transaction, executed command and other things we likely care about
  - It is entirely in memory
* Use [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to approve the log of actions run against the database

This approach means we can exercise all the features in DbUp for all target databases without ever touching a real database. This approach does not test that the database driver is setup correctly and that DbUp can actually run commands against that database. But this is easy to test manually, is unlikely to ever break and would be reported/fixed very quickly.
