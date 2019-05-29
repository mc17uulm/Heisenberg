% Diplomarbeitsvorlage, basierend auf ubicomp-long.cls von Stefan Dietzel
% (http://www.vf.utwente.nl/~dietzels/) und der Vorlage von Guido de Melo (http://guido.demelo.de)
% Zusammengestellt von Jonas Beinder (http://www.uni-ulm.de/in/mi/mitarbeiter/jonas-beinder.html)
%
% Ueberarbeitet/aufgeraeumt und einarbeitung der kiz vorlage durch
% Sebastian Maisch
% Christian van Onzenoodt
%
\ProvidesClass{thesis-uulm}[2017/07/11 v0.2 Diplomarbeitsvorlage Medieninformatik]

\RequirePackage[T1]{fontenc}
\usepackage[pagewise,modulo]{lineno}

\DeclareOption{uulm-draft}{
  \PassOptionsToPackage{backgroundcolor=orange}{todonotes}
  \PassOptionsToPackage{pdfborder={0 0 0.5}}{hyperref}
  \PassOptionsToPackage{final}{showkeys}
  \ClassWarning{myclass}{Draft Mode.}
  \linenumbers
}
\DeclareOption{uulm-draft-verbose}{
  \PassOptionsToPackage{backgroundcolor=orange}{todonotes}
  \PassOptionsToPackage{pdfborder={0 0 0.5}}{hyperref}
  \ClassWarning{myclass}{Draft Mode.}
  \linenumbers
}
\DeclareOption{uulm-release-electronic}{
  \PassOptionsToPackage{disable}{todonotes}
  \PassOptionsToPackage{pdfborder={0 0 0.5}}{hyperref}
  \PassOptionsToPackage{final}{showkeys}
}
\DeclareOption{uulm-release-print}{
  \PassOptionsToPackage{disable}{todonotes}
  \PassOptionsToPackage{pdfborder={0 0 0}}{hyperref}
  \PassOptionsToPackage{final}{showkeys}
}
\ProcessOptions\relax

\LoadClass[
	a4paper,	% A4
	titlepage,	% Mit Titelseite
	DIV=9,		% Text-Randverhältnis 6:3
	BCOR=10mm,	% Binding Correction
	twoside		% Zweiseitig
]{scrbook}

% Links im PDF
\RequirePackage[raiselinks=true,
    pdftex,
    colorlinks=false,
	bookmarks=true,
	bookmarksopenlevel=1,
	bookmarksopen=true,
	bookmarksnumbered=true,
	hyperindex=true,
	plainpages=false,
	pdfpagelabels=true,
	linkbordercolor={0 0.61 0.50},
    draft=false,
	citebordercolor={0 0.61 0.50}]{hyperref}
                        
\RequirePackage{iflang}
\RequirePackage{scrhack}
\RequirePackage[sc]{mathpazo}                 % Palatino
\RequirePackage{helvet}                       % Helvetica
\RequirePackage{booktabs}                     % Schönere Tables
\RequirePackage[final,draft=false]{microtype} % Micro-Typographische Verbesserungen
\RequirePackage{graphicx}                     % Grafiken
\RequirePackage{setspace}                     % Zeilenabstand
\RequirePackage[utf8]{inputenc}               % Codierung - Achtung! Wenn eine andere Codierung als UTF-8 verwendet wird, muss dies hier angegeben werden!
\RequirePackage[numbers]{natbib}              % Quellenformatierung
\RequirePackage{tikz}

\RequirePackage{todonotes}
\usepackage[color]{showkeys}

\def\normalsize{\@setfontsize{\normalsize}{10}{12.00pt}}	% 10 pt Schriftgröße
\normalsize
\abovedisplayskip 1.5ex plus4pt minus2pt
\belowdisplayskip \abovedisplayskip
\abovedisplayshortskip 0pt plus4pt
\belowdisplayshortskip 1.5ex plus4pt minus2pt

\RequirePackage[font=sf,sf]{caption}   	% immer Sans-Serif Font für Bildunterschriften
\RequirePackage[sf,SF]{subfigure}		

% Abstract page
\@ifpackagewith{babel}{ngerman}{\def\abstractname{Zusammenfassung}}{\def\abstractname{Abstract}}
\newenvironment{abstract}{
	\cleardoublepage
	\section*{\abstractname}
}
{\vfill\cleardoublepage}

% Variables
\def\@type{}
\newcommand\type[1]{\def\@type{#1}}

\def\@jahr{}
\newcommand\jahr[1]{\def\@jahr{#1}}

\def\@matnr{}
\newcommand\matnr[1]{\def\@matnr{#1}}

\def\@email{}
\newcommand\email[1]{\def\@email{#1}}

\def\@fakultaet{}
\newcommand\fakultaet[1]{\def\@fakultaet{#1}}

\def\@institut{}
\newcommand\institut[1]{\def\@institut{#1}}

\def\@gutachterA{}
\newcommand\gutachterA[1]{\def\@gutachterA{#1}}

\def\@gutachterB{}
\newcommand\gutachterB[1]{\def\@gutachterB{#1}}

\def\@betreuer{}
\newcommand\betreuer[1]{\def\@betreuer{#1}}

\def\@degree{}
\newcommand\degree[1]{\def\@degree{#1}}

% Titelseite
\renewcommand\maketitle{%
	\thispagestyle{empty}
	{	
		\begin{addmargin}[-4mm]{-24mm}
			\includegraphics[height=1.8cm]{template/unilogo_bild}
			\hfill
			\includegraphics[height=1.8cm]{template/unilogo_wort}

			\vspace{.5cm}
				\begin{flushright}
                    \IfLanguageName{ngerman}{
					\textsf{\bfseries Universität Ulm \textbar \mdseries ~89069 Ulm \textbar~Germany}\\\vspace{1.25cm}
					\bfseries Fakultät für Ingenieurwissenschaften, \\Informatik und Psychologie\\ \mdseries Institut für Medieninformatik\\ \mdseries Forschungsgruppe Mensch-Computer-Interaktion
                    }{
					\textsf{\bfseries Ulm University \textbar \mdseries ~89069 Ulm \textbar~Germany}\\\vspace{1.25cm}
					\bfseries Faculty of Engineering, \\Computer Science and Psychology\\ \mdseries Institute of Media Informatics\\ \mdseries Human-Computer-Interaction Group
                    }
				\end{flushright}
			\vspace{2cm}
            \Huge\bfseries\@title\\[.25cm]
			\normalsize
			\mdseries\@type~in~\@degree~\IfLanguageName{ngerman}{an der Universität Ulm}{at Ulm University}\\[1cm]
			\bfseries \IfLanguageName{ngerman}{Vorgelegt von}{Presented by}:\\
			\mdseries\@author\\
			\@email\\[2.5cm]
			\bfseries \IfLanguageName{ngerman}{Gutachter}{Examiner}:\\
			\mdseries\@gutachterA\\
			\@gutachterB\\[.5cm]
			\bfseries \IfLanguageName{ngerman}{Betreuer}{Advisor}:\\
			\mdseries\@betreuer\\[1cm]
			\mdseries\@jahr
		\end{addmargin}
	}
}

% Copyright page
\newcommand\copyrightinfo{
	\copyright~\@jahr~\@author\\[.5em]
    \IfLanguageName{ngerman}{
    Diese Arbeit ist lizenziert unter der Creative Commons
	\textbf{Namensnennung-Keine kommerzielle Nutzung-Weitergabe unter gleichen Bedingungen
	3.0 Deutschland} Lizenz. Nähere Informationen finden Sie unter
	\url{http://creativecommons.org/licenses/by-nc-sa/3.0/de/}.
    }{
    This work is licensed under the Creative Commons \textbf{Attribution-NonCommercial-ShareAlike 3.0 Unported} License.
    To view a copy of this license, visit \url{http://creativecommons.org/licenses/by-nc-sa/3.0/}.
    }
}

% Declaration page
\newcommand\declaration{
	\ihead{\IfLanguageName{ngerman}{Name}{Name}: \@author}
	\ohead{\IfLanguageName{ngerman}{Matrikelnummer}{Matriculation Number}: \@matnr}
	\ofoot{\pagemark}	
	\vspace*{2cm}	
	\minisec{\IfLanguageName{ngerman}{Erklärung}{Declaration}}
	\vspace{.5cm}
    \IfLanguageName{ngerman}{
    Hiermit versichere ich, dass ich die vorliegende Arbeit mit dem Titel:
    \begin{center}
    \bfseries\@title \\[1em]
    \end{center}
    bis auf die offizielle Betreuung selbstständig und ohne fremde Hilfe angefertigt habe und die benutzten Quellen und Hilfsmittel vollständig angegeben sind.
    Aus fremden Quellen direkt oder indirekt übernommenen Gedanken sind jeweils unter Angabe der Quelle als solche kenntlich gemacht.
    Ich erkläre außerdem, dass die vorliegende Arbeit entsprechend den Grundsätzen guten wissenschaftlichen Arbeitens gemäß der "Satzung der Universität Ulm zur Sicherung guter wissenschaftlicher Praxis" erstellt wurde.}{
    I hereby declare that this thesis titled:
    \begin{center}
    \bfseries\@title \\[1em]
    \end{center}
    is the product of my own independent work and that I have used no sources or materials other than those specified. The passages taken from other works, either verbatim or paraphrased in the spirit of the original quote, are identified in each individual case by indicating the source.
    I further declare that all my academic work was written in line with the principles of proper academic research according to the official “Satzung der Universität Ulm zur Sicherung guter wissenschaftlicher Praxis” (University Statute for the Safeguarding of Proper Academic Practice).}\\[1cm]
	\hspace*{2cm} \IfLanguageName{ngerman}{Ulm, den}{Ulm,} \dotfill\\
	\hspace*{10cm} {\footnotesize \@author}
}

\newcommand\copyrightpage{
    \clearpage
    \thispagestyle{empty}
    {	\small
        \flushleft
        ~\vfill
        \textsf{\IfLanguageName{ngerman}{Fassung vom}{Last updated} \today\\[1cm]
        \copyrightinfo\\[.5cm]
        \IfLanguageName{ngerman}{Satz}{Typesetting}: PDF-\LaTeXe}
    }
}

\RequirePackage{scrpage2}
\pagestyle{scrheadings}
\clearscrheadfoot
\automark[section]{chapter}

\rohead{\MakeUppercase{\rightmark}}
\lehead{\MakeUppercase{\leftmark}}
\ofoot{\pagemark}

\setkomafont{pageheadfoot}{%
\normalfont\sffamily\scshape\footnotesize
}
\setkomafont{pagenumber}{%
\normalfont\sffamily\footnotesize
}

% Schusterjungen / Hurenkinder vermeiden
\widowpenalty10000
\clubpenalty10000

% Bis zu welcher Tiefe kommen Überschriften ins Inhaltsverzeichnis
\setcounter{tocdepth}{2}

