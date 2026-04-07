// recip-ez.client\src\pages\Inventory.jsx
import InventoryAdditionCard from "../components/ui/inventory/InventoryAdditionCard"
import  Grid from '@mui/material/Grid'

export default function Inventory() {
    return (
        <>
            <Grid container spacing={2}>
                <Grid size={3}>
                    <InventoryAdditionCard/>
                </Grid>
                <Grid size={5}>
                    <h1>Inventory Space</h1>
                </Grid>
            </Grid>
        </>
    )
}