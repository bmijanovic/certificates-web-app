import React, {useState} from "react";
import {useQuery} from "@tanstack/react-query";
import {Box, Card, CardActions, CardContent, Grid, Modal, TextField, Typography} from "@mui/material";
import Button from "@mui/material/Button";
import axios from "axios";
import {useNavigate} from "react-router-dom";
import {Cancel, Download} from "@mui/icons-material";

export default function CertificateCard(props) {
    const certificate={endDate:props.data.endDate,
    ownerAttributes:props.data.ownerAttributes,
    issuerAttributes:props.data.issuerAttributes,
    isValid:props.data.isValid,
    serialNumber:props.data.serialNumber,
    startDate:props.data.startDate,
    certificateType:props.data.certificateType}
    const flagsNames = ["EncipherOnly", "CrlSign", "KeyCertSign", "KeyAgreement", "DataEncipherment", "KeyEncipherment", "NonRepudiation", "DigitalSignature"]
    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
    const navigate = useNavigate()

    function makeStringOfFlags(flagsStr) {
        let flagsNumbers = flagsStr.split(",");
        let str = ""
        for(let flagNum in flagsNumbers)
            str += flagsNames[flagNum] + '\n';
        return str
    }

    function formatDate(date){
        date=date.split('T')[0].split('-')

        return "".concat(date[2],".",date[1],".",date[0],".")
    }
    function downloadCertificate(){
        fetch(`https://localhost:7018/api/Certificate/download/${certificate.serialNumber}`).then((res) => {
            return res.blob();
        }).then(blob => {
            const link = document.createElement('a');
            const url = URL.createObjectURL(blob);
            console.log(url);
            link.href = url;
            link.download = `${certificate.serialNumber}.crt`;
            link.click();
        });
    }
    function downloadKeyCertificate(){
        fetch(`https://localhost:7018/api/Certificate/download/key/${certificate.serialNumber}`).then((res) => {
            return res.blob();
        }).then(blob => {
            const link = document.createElement('a');
            const url = URL.createObjectURL(blob);
            console.log(url);
            link.href = url;
            link.download = `${certificate.serialNumber}.key`;
            link.click();
        });
    }
    const ownershipQuery = useQuery({
        queryKey: ["certificateOwnership"],
        queryFn: () => axios.get(`https://localhost:7018/api/Certificate/ownership/${certificate.serialNumber}`).then(res => res.data).catch(err => {console.log(err)})});

    const style = {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 400,
        bgcolor: 'background.paper',
        boxShadow: 24,
        borderRadius:3,
        p: 4,
    };



    return <>
        <Modal
            open={open}
            onClose={handleClose}>
            <Box sx={style}>
                <div style={{width:180, height:180, backgroundColor:"#146C94", margin:"0 auto", borderRadius:"20px"}}></div>

                <Typography variant="h5" component="h3" style={{textAlign:"center",margin:"0 auto", color:"#146C94"}}>
                    <strong>{certificate.ownerAttributes.split(';').find(s => s.split('=')[0] === 'CN')?.split('=')[1]}</strong>
                </Typography>
                <Typography variant="h6" component="h2" style={{textAlign:"center", marginBottom:5}}>
                    {`${certificate.certificateType.toString()[0].toUpperCase()}${certificate.certificateType.toString().substring(1).toLowerCase()}`}
                </Typography>
                <div  style={{height:1, width: 230,backgroundColor:"rgba(0,0,0,0.3)",margin:"0 auto",marginBottom:20}}></div>
                <Typography variant="h6" color="textSPrimary" textAlign="center" gutterBottom>
                    {formatDate(certificate.startDate)} - {formatDate(certificate.endDate)}
                </Typography>
                <Typography color="textPrimary" textAlign="center" gutterBottom>
                    <span style={{marginBottom:20  }}>Serial Number:</span><br/>
                    <strong style={{fontSize:20,color:"#146C94"}}>{certificate.serialNumber}</strong>
                </Typography>
                <div  style={{height:1, width: 230,backgroundColor:"rgba(0,0,0,0.3)",margin:"0 auto",marginBottom:20}}></div>
                {certificate.issuerAttributes!==""&&
                <Typography color="textPrimary" textAlign="center" gutterBottom>
                    <span style={{marginBottom:20  }}>Issuer</span><br/>
                    <strong style={{fontSize:20,color:"#146C94"}}>{certificate.issuerAttributes}</strong>
                </Typography>}
                <Typography color="textPrimary" textAlign="center" gutterBottom>
                    <span style={{marginBottom:20  }}>Owner</span><br/>
                    <strong style={{fontSize:20,color:"#146C94"}}>{certificate.ownerAttributes.split(';').find(s => s.split('=')[0] === 'CN')?.split('=')[1]}</strong>
                </Typography>
                <div style={{display:"flex", justifyContent:"center"}}>
                    <Button fullWidth variant="contained" color="primary" startIcon={<Download/>}  onClick={downloadCertificate} style={{padding:5,marginLeft:1,marginRight:1}} >Certificate</Button>
                    {ownershipQuery.data===true&&
                    <Button fullWidth variant="contained" color="primary" startIcon={<Download/>}  onClick={downloadKeyCertificate} style={{padding:5,marginLeft:1,marginRight:1}} >Key</Button>
                    }
                    {ownershipQuery.data===true&&
                        <Button fullWidth variant="contained" color="primary" startIcon={<Cancel/>} onClick={downloadCertificate} style={{padding:5,marginLeft:1,marginRight:1}} >Withdraw</Button>
                    }
                </div>
            </Box>
        </Modal>
        <Grid item xs={12} sm={6} md={4}>
            <div style={{ display: "flex", alignItems: "center", flexDirection:"column"}}>
                <div style={{width:180, height:180, backgroundColor:"#146C94", position:"relative", top:75, borderRadius:"20px"}}></div>
                <Card>
                    <CardContent style={{height:200, width: 280, marginTop: 80,paddingTop:0}}>
                        <Typography variant="h5" component="h3" style={{textAlign:"center", marginBottom:0, color:"#146C94"}}>
                            <strong>{certificate.ownerAttributes.split(';').find(s => s.split('=')[0] === 'CN')?.split('=')[1]}</strong>
                        </Typography>
                        <Typography variant="h6" component="h2" style={{textAlign:"center", marginBottom:5}}>
                            {`${certificate.certificateType.toString()[0].toUpperCase()}${certificate.certificateType.toString().substring(1).toLowerCase()}`}
                        </Typography>
                        <div  style={{height:1, width: 230,backgroundColor:"rgba(0,0,0,0.3)",margin:"0 auto",marginBottom:20}}></div>
                        <Typography variant="h6" color="textSPrimary"  gutterBottom>
                            {formatDate(certificate.startDate)} - {formatDate(certificate.endDate)}
                        </Typography>
                        <Typography color="textPrimary" gutterBottom>
                            <span style={{marginBottom:20  }}>Serial Number:</span><br/>
                            <strong style={{fontSize:20,color:"#146C94"}}>{certificate.serialNumber}</strong>
                        </Typography>
                       </CardContent>
                    <CardActions style={{padding:10}}>
                        <Button fullWidth variant="contained" color="primary" sx={{m:2}} onClick={handleOpen} style={{padding:2,margin:"0 auto"}} >View Details</Button>
                    </CardActions>

                </Card>
            </div>
        </Grid>
    </>
}