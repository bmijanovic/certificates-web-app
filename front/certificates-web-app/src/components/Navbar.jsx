import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import axios from "axios";
import {useNavigate} from "react-router-dom";
import {environment} from "../security/Environment.jsx";
import {useContext} from "react";
import {AuthContext} from "../security/AuthContext.jsx";
export default function Navbar() {
    const { isAuthenticated, isTwoFactorVerified,
        isPasswordExpired } = useContext(AuthContext);

    const [anchorElNav, setAnchorElNav] = React.useState(null);
    const [anchorElUser, setAnchorElUser] = React.useState(null);
    const [value, setValue] = React.useState(0);
    const navigate = useNavigate()
    const handleOpenNavMenu = (event) => {
        setAnchorElNav(event.currentTarget);
    };
    const handleOpenUserMenu = (event) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleCloseUserMenu = () => {
        setAnchorElUser(null);
    };

    const handleCertificatesClick = (event) => {
        handleCloseNavMenu(event)
        navigate('/certificates')
    };

    const handleRequestsClick = (event) => {
        handleCloseNavMenu(event)
        navigate('/requests')
    };

    const handleGenerateClick = (event) => {
        handleCloseNavMenu(event)
        navigate('/generate')
    };

    const handleValidityClick = (event) => {
        handleCloseNavMenu(event)
        navigate('/checkValidity')
    };

    const handleLogoutClick = (event) => {
        handleCloseNavMenu(event)
        event.preventDefault()

        axios.post(environment + `/api/User/logout`)
            .then(res => {
                if (res.status === 200){
                    navigate(0);
                }
            }).catch((error) => {
            console.log(error);
        });
    };




    return (
        <AppBar style={{position:"sticky",margin:0}}>
            <Container maxWidth="xl">
                <Toolbar disableGutters>

                    <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <MenuIcon />
                        </IconButton>
                        <Menu
                            id="menu-appbar"
                            anchorEl={anchorElNav}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'left',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'left',
                            }}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                            sx={{
                                display: { xs: 'block', md: 'none' },
                            }}
                        >
                            <MenuItem key="certificates" onClick={handleCertificatesClick}>
                                <Typography textAlign="center">Certificates</Typography>
                            </MenuItem>
                            <MenuItem key="requests" onClick={handleRequestsClick}>
                                <Typography textAlign="center">Requests</Typography>
                            </MenuItem>
                            <MenuItem key="generate" onClick={handleGenerateClick}>
                                <Typography textAlign="center">Create Certificates</Typography>
                            </MenuItem>
                            <MenuItem key="validity" onClick={handleValidityClick}>
                                <Typography textAlign="center">Check Validity</Typography>
                            </MenuItem>
                            <MenuItem key="logout" onClick={handleLogoutClick}>
                                <Typography textAlign="center">Logout</Typography>
                            </MenuItem>
                        </Menu>
                    </Box>
                    {isTwoFactorVerified && !isPasswordExpired &&
                        <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                            <Button
                                key="certificates"
                                onClick={handleCertificatesClick}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                Certificates
                            </Button>
                            <Button
                                key="requests"
                                onClick={handleRequestsClick}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                Requests
                            </Button>
                            <Button
                                key="generate"
                                onClick={handleGenerateClick}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                Create Certificate
                            </Button>
                            <Button
                                key="validity"
                                onClick={handleValidityClick}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                Check Validity
                            </Button>
                        </Box>
                    }

                    <Box sx={{ flexGrow: 0,display: { xs: 'none', md: 'flex' }  }}>
                        <Button
                            key="logout"
                            onClick={handleLogoutClick}
                            sx={{ my: 2, color: 'white', display: 'block' }}
                        >
                            Logout
                        </Button>
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}