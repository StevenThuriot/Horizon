---
title: "contribute"
bg: slategray
color: white
fa-icon: cloud-upload
---

## Contributing to Horizon

Want to hack on Horizon? Awesome! We'd love for you to contribute to our source code and to make Horizon even better than it is today!

-------------------------


### Got a Question or Problem?

If you have questions about how to use Horizon, feel free to contact through [GitHub][issues].

### Found an Issue?

If you find a bug in the source code or a mistake in the documentation, you can help us by submitting an issue to our [GitHub Repository][github]. 

Even better, you can submit a Pull Request with a fix.

-------------------------


### Want a Feature?

You can request a new feature by submitting an issue to our [GitHub Repository][github]. 

Pull Request will also be accepted!

-------------------------


### Submitting a Pull Request
Before you submit your pull request consider the following:

Search [GitHub][pulls] for an open or closed Pull Request that relates to your submission. You don't want to duplicate effort.

Make your changes in a new git branch:

```
git checkout -b my-fix-branch master
```

Commit your changes using a descriptive commit message.

```
git commit -a
```

Note: the optional commit `-a` command line option will automatically "add" and "rm" edited files.

Push your branch to GitHub:

```
git push origin my-fix-branch
```

In GitHub, send a pull request to `Horizon:master`.
If we suggest changes then:
* Make the required updates.
* Rebase your branch and force push to your GitHub repository (this will update your Pull Request):

```
git rebase master -i
git push origin my-fix-branch -f
```

That's it! Thank you for your contribution!

#### After your pull request is merged

After your pull request is merged, you can safely delete your branch and pull the changes
from the main (upstream) repository:

Delete the remote branch on GitHub either through the GitHub web UI or your local shell as follows:

```
git push origin --delete my-fix-branch
```

Check out the master branch:

```
git checkout master -f
```

Delete the local branch:

```
git branch -D my-fix-branch
```

Update your master with the latest upstream version:

```
git pull --ff upstream master
```

[github]: {{ site.source_link }}
[issues]: {{ site.source_link }}/issues
[pulls]: {{ site.source_link }}/pulls
