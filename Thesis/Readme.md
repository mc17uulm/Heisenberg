Usage:

Adjust variables in `thesis.tex` with our informations.
Make sure to adjust the following variables:

- language   / choose either ngerman or english depending on if your are writing your thesis in english or german.
- author     / your full name
- title      / title of the thesis
- email      / our university email adress
- matnr      / your student-id (matrikelnummer)
- degree     / the degree you are enrolled in (media informatics, computer science, cognitive systems, ...)
- type       / (master/ bachelor thesis)
- jahr       / the current year (and month)
- gutachterA / your first examiner
- gutachterB / in case of a master thesis your second examiner (for bachelor thesis comment this out)
- betreuer   / name of your advisor

You can add additional chapters by adding a new tex-file to the `chapters` directory and adding this new file to the `thesis.tex` file in the section after `mainmatter` (around line 74) and into the
`includeonly` section (around line 41).

If you just want to include specific chapters, you can disable them by comment them out of the `includeonly` section.
