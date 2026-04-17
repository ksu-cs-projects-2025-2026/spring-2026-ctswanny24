// recip-ez.client\src\pages\Inventory.jsx
import InventoryAdditionCard from "../components/ui/inventory/InventoryAdditionCard"
import  Grid from '@mui/material/Grid'
import InventoryItemCard from "../components/ui/inventory/InventoryItemCard"
import AddInventoryItem from "../components/ui/inventory/AddToInventory"

export default function Inventory() {
    return (
        <>
            <Grid container spacing={2}>
                <Grid size={4}>
                    <AddInventoryItem/>
                </Grid>
                <Grid container spacing={2}>
                    <Grid size={10}>
                    <InventoryItemCard/>
                    </Grid>
                </Grid>
            </Grid>
        </>
    )
}