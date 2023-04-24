import React from "react";
import {useQuery} from "@tanstack/react-query";
import {Card, CardContent, Grid, Typography} from "@mui/material";

export default function CertificateRequestCard({data}) {
    const { id, type, state, hashAlgorithm, endDate, flags, parentSerialNumber } = data;

    return <>
        <Grid item xs={12} sm={6} md={4}>
            <Card>
                <CardContent style={{textAlign:"left"}}>
                    <Typography variant="h5" component="h2">
                        {parentSerialNumber === "" ? "-" : parentSerialNumber}

                    </Typography>
                    <Typography color="textSecondary" gutterBottom>
                        Type: {type}
                    </Typography>
                    <Typography color="textSecondary" gutterBottom>
                        State: {state}
                    </Typography>
                    <Typography color="textSecondary" gutterBottom>
                        Hash Algorithm: {hashAlgorithm}
                    </Typography>
                    <Typography color="textSecondary" gutterBottom>
                        End Date: {endDate}
                    </Typography>
                    <Typography color="textSecondary" gutterBottom>
                        Flags: {flags}
                    </Typography>
                </CardContent>
            </Card>
        </Grid>
    </>
}