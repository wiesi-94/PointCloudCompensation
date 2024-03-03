# PointCloudCompensation
Applys error compensation to point clouds taken with a laser scanner (TLS).

The program can read PTX and Faro FLS, applies systemic error compensation, and outputs PTX files.
Currently only the gray values are taken from the FLS file, no colors.
All additional sensor values from the FLS file are lost.

Most calculations are from the work "Accuracy assessment of the FARO Focus3D and Leica HDS6100 panoramictype terrestrial laser scanners through point-based and plane-based user
self-calibration" by Jacky C.K. CHOW, Derek D. LICHTI, and William F. TESKEY.

To use the FLS import, a Faro license key and the Faro SDK are required.
