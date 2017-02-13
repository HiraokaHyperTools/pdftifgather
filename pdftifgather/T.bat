SET C=bin\x86\DEBUG\pdftifgather.exe
%C% Outs\0-t.tif ( Sample.tif 1 ) || PAUSE
%C% Outs\90-t.tif ( Sample.tif 1right ) || PAUSE
%C% Outs\180-t.tif ( Sample.tif 1down ) || PAUSE
%C% Outs\270-t.tif ( Sample.tif 1left ) || PAUSE
%C% Outs\0-p.tif ( Sample.pdf 1 ) || PAUSE
%C% Outs\90-p.tif ( Sample.pdf 1r ) || PAUSE
%C% Outs\180-p.tif ( Sample.pdf 1d ) || PAUSE
%C% Outs\270-p.tif ( Sample.pdf 1l ) || PAUSE

%C% Outs\0-t.pdf ( Sample.tif 1 ) || PAUSE
%C% Outs\90-t.pdf ( Sample.tif 1r ) || PAUSE
%C% Outs\180-t.pdf ( Sample.tif 1d ) || PAUSE
%C% Outs\270-t.pdf ( Sample.tif 1l ) || PAUSE
%C% Outs\0-p.pdf ( Sample.pdf 1 ) || PAUSE
%C% Outs\90-p.pdf ( Sample.pdf 1r ) || PAUSE
%C% Outs\180-p.pdf ( Sample.pdf 1d ) || PAUSE
%C% Outs\270-p.pdf ( Sample.pdf 1l ) || PAUSE

PAUSE