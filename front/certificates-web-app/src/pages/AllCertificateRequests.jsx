import React, {useContext, useState} from "react";
import {useQuery} from "@tanstack/react-query";
import CertificateRequestCard from "../components/CertificateRequestCard.jsx";
import axios from "axios";
import {Box, Grid, Tab, Tabs, Typography} from "@mui/material";
import { TabPanel, TabContext } from '@mui/lab';
import {AuthContext} from "../security/AuthContext.jsx";



export default function AllCertificateRequests() {
    const [value, setValue] = useState(0);
    const { isAuthenticated, role, isLoading } = useContext(AuthContext);


    const certificateRequestsQuery = useQuery({
        queryKey: ["certificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest").then(res => res.data).catch(err => {console.log(err)})
    })

    const allCertificateRequestsQuery = useQuery({
        queryKey: ["allCertificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest/getAll").then(res => res.data).catch(err => {console.log(err)})
    })

    const approvalCertificateRequestsQuery = useQuery({
        queryKey: ["approvalCertificateRequest"],
        queryFn: () => axios.get("https://localhost:7018/api/CertificateRequest/forApproval").then(res => res.data).catch(err => {console.log(err)})
    })

    const handleChange = (event, newValue) => {
        setValue(newValue);
    };

    const renderPanel = (index) => {
        switch (index) {
            case 0:
                return <>
                    {certificateRequestsQuery.isLoading ? <p>Loading...</p> : <div>
                        <Grid container sx={{bx:3, mt:1}} spacing={5}>
                            {certificateRequestsQuery.data.map(item => <CertificateRequestCard key={item.id} data={item} acceptable={false}/>)}
                        </Grid>
                    </div>
                    }
                </>
            case 1:
                return <>
                    {approvalCertificateRequestsQuery.isLoading ? <p>Loading...</p> : <div>
                        <Grid container sx={{bx:3, mt:1}} spacing={5}>
                            {approvalCertificateRequestsQuery.data.map(item => <CertificateRequestCard key={item.id} data={item} acceptable={true}/>)}
                        </Grid>
                    </div>
                    }
                </>
            case 2:
                return <>
                    {allCertificateRequestsQuery.isLoading ? <p>Loading...</p> : <div>
                        <Grid container sx={{bx:3, mt:1}} spacing={5}>
                            {allCertificateRequestsQuery.data.map(item => <CertificateRequestCard key={item.id} data={item}  acceptable={false}/>)}
                        </Grid>
                    </div>
                    }
                </>
            default:
                return null;
        }
    };


    return <>
        <div style={{textAlign: "center", width: "80%", margin:"auto"}}>
            <h1>Certificate Requests</h1>
            <TabContext value={value.toString()}>
                <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
                    <Tabs variant={"fullWidth"} value={value} onChange={handleChange} aria-label="basic tabs example">
                        <Tab label="Requests for your certificates"/>
                        <Tab label="Request based on your certificates"/>
                        {role === "Admin" ? <Tab label="All requests" /> : null}
                    </Tabs>
                </Box>
                {renderPanel(value)}

            </TabContext>
        </div>
    </>
}