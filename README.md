# pdftifgather
PDF/TIFF gather (GPLv3)

	pdftifgather out.pdf in1.pdf in2.pdf in3.pdf
	pdftifgather out.pdf ( in-even.pdf 2 4 6 ) ( in-odd.pdf 1 3 5 )
	pdftifgather out.tif ( in-even.tif 2 4 6 ) ( in-odd.tif 1 3 5 )
	pdftifgather       out.tif ( in.tif 1L 1Left 1R 1Right 1D 1Down )
	pdftifgather /v0.4 out.tif ( in.tif 1L 1Left 1R 1Right 1D 1Down )
	pdftifgather out.tif ( in.tif 1- )
	pdftifgather out.tif ( in.tif 2-3 )
	pdftifgather out.tif ( in.tif 4- )
	pdftifgather /GPC in.tif → %ERRORLEVEL%
	pdftifgather /GPC in.pdf → %ERRORLEVEL%

* Requires .NET Framework 4.0 or later
